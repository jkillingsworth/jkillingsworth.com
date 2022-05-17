#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

show_help()
{
    echo "Usage: $(basename ${0}) [OPTION]... [TEXFILE [SVGFILE]]"
}

#--------------------------------------------------------------------------------------------------

convert_tex_to_dvi()
{
    set +e
    clargs="--halt-on-error --interaction=nonstopmode"
    output=$(latex ${clargs} --output-dir="$(pwd)" "${jobname}.tex")
    result=${?}
    set -e

    if [ ${result} != 0 ]; then
        echo "${output}" 1>&2
        exit 1
    fi
}

convert_dvi_to_svg()
{
    clargs="${1} --exact --zoom=1.333333 --precision=6 --verbosity=3"
    output=$(dvisvgm ${clargs} --output="${jobname}.svg" "${jobname}.dvi" 2>&1)

    if [ -n "${output}" ]; then
        echo "${output}" 1>&2
        exit 1
    fi
}

#--------------------------------------------------------------------------------------------------

do_autohint()
{
    fn_previous=${1}
    fn_autohint=${2}

    clargs="--no-info"

    set +e
    ttfautohint ${clargs} ${fn_previous} ${fn_autohint} 2> /dev/null
    try_symbol_font=$?
    set -e

    if [ ${try_symbol_font} != 0 ]; then
        ttfautohint ${clargs} --symbol ${fn_previous} ${fn_autohint}
    fi
}

do_nulldate()
{
    fn_previous=${1}
    fn_nulldate=${2}

    start="<head>"
    end="<\/head>"
    pattern="<(created|modified) value=\"(.+)\"\/>"
    replace="<\1 value=\"Thu Jan 01 00:00:00 1970\"\/>"
    ex_head="/${start}/,/${end}/ s/${pattern}/${replace}/"

    start="<name>"
    end="<\/name>"
    pattern="FontForge 2.0 : (.*) : [0-9]{1,2}-[0-9]{1,2}-[0-9]{4}"
    replace="FontForge 2.0 : \1 : 1-1-1970"
    ex_name="/${start}/,/${end}/ s/${pattern}/${replace}/"

    fn_begin="${prefix}-ttx-begin.xml"
    fn_final="${prefix}-ttx-final.xml"
    ttx -qei -x FFTM --newline=LF -o ${fn_begin} ${fn_previous}
    sed -E "${ex_head};${ex_name}" ${fn_begin} > ${fn_final}
    ttx -qb --no-recalc-timestamp -o ${fn_nulldate} ${fn_final}
}

do_compress()
{
    fn_previous=${1}
    fn_compress=${2}

    "${basedir}"/bin/fontpp ${fn_previous} ${fn_compress}
}

#--------------------------------------------------------------------------------------------------

ttfont_set_filenames()
{
    i=${1}

    printf -v prefix "ttfont-%02i" ${i}
    fn_original="${prefix}-original.txt"
    fn_decoding="${prefix}-decoding.ttf"
    fn_autohint="${prefix}-autohint.ttf"
    fn_nulldate="${prefix}-nulldate.ttf"
    fn_compress="${prefix}-compress.woff2"
    fn_encoding="${prefix}-encoding.txt"
}

ttfont_process_fork()
{
    i=${1}
    ttfont_set_filenames ${i}

    echo "${ttfonts[${i}]}" > ${fn_original}
    base64 --decode ${fn_original} > ${fn_decoding}
    do_autohint ${fn_decoding} ${fn_autohint}
    do_nulldate ${fn_autohint} ${fn_nulldate}
    do_compress ${fn_nulldate} ${fn_compress}
    base64 --wrap=0 ${fn_compress} > ${fn_encoding}
}

ttfont_process_join()
{
    i=${1}
    ttfont_set_filenames ${i}

    original=$(< ${fn_original})
    encoding=$(< ${fn_encoding})
    svgfile=${svgfile/${original}/${encoding}}
}

#--------------------------------------------------------------------------------------------------

do_post_processing()
{
    upper_pattern="(?s)^(.+)(<\!\[CDATA\[)"
    inner_pattern="(?s)(?<=<\!\[CDATA\[)(.+)(?=]]>)"
    lower_pattern="(?s)(\n]]>)(.+)$"

    upper=$(grep -oPz "${upper_pattern}" ${jobname}.svg | tr -d "\0")
    inner=$(grep -oPz "${inner_pattern}" ${jobname}.svg | tr -d "\0" | sort)
    lower=$(grep -oPz "${lower_pattern}" ${jobname}.svg | tr -d "\0")

    printf -v svgfile "$(sed -E s/%/%%/ <<< "${upper}\n${inner}${lower}")"

    pattern="(?<=src:url\(data:application/x-font-ttf;base64,)(.+)(?=\) format\('truetype'\);)"
    ttfonts=$(grep -oP "${pattern}" <<< "${svgfile}")
    ttfonts=(${ttfonts})

    for i in ${!ttfonts[@]}; do
        ttfont_process_fork ${i} &
    done

    wait

    for i in ${!ttfonts[@]}; do
        ttfont_process_join ${i}
    done

    pattern="<!-- (.*) -->"
    replace="<!-- \1 (modified) -->"
    ex_domain="2 s/${pattern}/${replace}/"

    pattern="application\/x-font-ttf"
    replace="application\/x-font-woff2"
    ex_medium="s/${pattern}/${replace}/"

    pattern="format\('truetype'\)"
    replace="format\('woff2'\)"
    ex_format="s/${pattern}/${replace}/"

    svgfile=$(sed -E "${ex_domain};${ex_medium};${ex_format}" <<< ${svgfile})

    conversion=cat
    case "$(uname -s)" in CYGWIN*|MSYS*|MINGW* ) conversion=unix2dos ;; esac
    echo "${svgfile}" | ${conversion} > "${jobname}.svg"
}

#--------------------------------------------------------------------------------------------------

option_fonts=nofonts

options=$(getopt -n "${0}" -o f: -l fonts:,help -- "${@}")

eval set -- "${options}"

while true; do case "${1}" in
    -f | --fonts )
        option_fonts="${2}"
        shift 2
        ;;
    --help )
        show_help
        exit 0
        ;;
    -- )
        shift 1
        break
        ;;
    * )
        echo -e "$(basename ${0}): unhandled case -- ${1}\a" 1>&2
        exit 1
        ;;
esac done

if [ -t -0 ] && [ -z "${1}" ]; then
    show_help
    exit 1
fi

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)
tempdir=$(mktemp -d)
jobname="latextosvg"

cat "${1:--}" > "${tempdir}/${jobname}.tex"

pushd "${tempdir}" > /dev/null

case "${option_fonts}" in
    nofonts )
        convert_tex_to_dvi
        convert_dvi_to_svg "--no-fonts"
        ;;
    ttfonts )
        convert_tex_to_dvi
        convert_dvi_to_svg "--font-format=ttf --bbox=0.5625bp"
        do_post_processing
        ;;
    * )
        echo -e "$(basename ${0}): invalid font option -- ${option_fonts}" 1>&2
        exit 1
        ;;
esac

popd > /dev/null

if [ -n "${2}" ]; then
    cat "${tempdir}/${jobname}.svg" > "${2}"
else
    cat "${tempdir}/${jobname}.svg"
fi

rm -rf "${tempdir}"
