#!/bin/bash

set -e

show_help() {
    echo "Usage: $(basename ${0}) [OPTION]... [TEXFILE [SVGFILE]]"
}

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

basedir=$(realpath "${0}" | xargs -0 dirname)
jobname="latextosvg"

convert_tex_to_dvi() {

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

convert_dvi_to_svg() {

    clargs="${1} --exact --zoom=1.333333 --precision=6 --verbosity=3"
    output=$(dvisvgm ${clargs} "${jobname}.dvi" 2>&1)

    if [ -n "${output}" ]; then
        echo "${output}" 1>&2
        exit 1
    fi
}

do_autohint () {
    fn_native=${1}
    fn_hinted=${2}

    clargs="--no-info"

    set +e
    ttfautohint ${clargs} ${fn_native} ${fn_hinted} 2> /dev/null
    try_symbol_font=$?
    set -e

    if [ ${try_symbol_font} != 0 ]; then
        ttfautohint ${clargs} --symbol ${fn_native} ${fn_hinted}
    fi
}

do_compress () {
    fn_hinted=${1}
    fn_output=${2}

    "${basedir}/fontpp" ${fn_hinted} ${fn_output}
}

post_process_fonts () {

    pattern="(?<=src:url\(data:application/x-font-ttf;base64,)(.+?)(?=\) format\('truetype'\);)"
    svgfile=$(< ${jobname}.svg)
    ttfonts=$(grep -oP "${pattern}" <<< "${svgfile}")

    for native in ${ttfonts}; do
        fn_native="tt_native.ttf"
        fn_hinted="tt_hinted.ttf"
        fn_output="tt_output.woff2"
        base64 --decode <<< ${native} > ${fn_native}
        do_autohint ${fn_native} ${fn_hinted}
        do_compress ${fn_hinted} ${fn_output}
        output=$(base64 --wrap=0 ${fn_output})
        svgfile=${svgfile/${native}/${output}}
        rm ${fn_native}
        rm ${fn_hinted}
        rm ${fn_output}
    done

    svgfile=${svgfile//"application/x-font-ttf"/"application/x-font-woff2"}
    svgfile=${svgfile//"format('truetype')"/"format('woff2')"}

    upper_pattern="^(.|\n)+?(<\!\[CDATA\[)"
    inner_pattern="(?<=<\!\[CDATA\[)(.|\n)+?(?=]]>)"
    lower_pattern="(\n]]>)(.|\n)+?$"

    upper=$(grep -oPz "${upper_pattern}" <<< "${svgfile}" | tr -d "\0")
    inner=$(grep -oPz "${inner_pattern}" <<< "${svgfile}" | tr -d "\0" | sort)
    lower=$(grep -oPz "${lower_pattern}" <<< "${svgfile}" | tr -d "\0")

    svgfile="${upper}${inner}${lower}"

    conversion=cat
    case "$(uname -s)" in CYGWIN*|MSYS*|MINGW* ) conversion=unix2dos ;; esac
    echo "${svgfile}" | ${conversion} > "${jobname}.svg"
}

tempdir=$(mktemp -d)

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
        post_process_fonts
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
