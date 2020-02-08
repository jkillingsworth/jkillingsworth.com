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
        convert_dvi_to_svg "--font-format=ttf"
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