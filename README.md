## Overview

This is the source repository for my personal website.

## Setup

The build the site, you'll need to install [Ruby](https://www.ruby-lang.org/) and use the [Jekyll](https://jekyllrb.com/) static site generator. Several additional packages are also required. The following instructions are designed for installing the required toolchain on a 64-bit Windows system. These tools are available on other platforms as well.

1. Download and run the [MSYS2](https://www.msys2.org/) installer. Follow the installation instructions documented on the website. Make sure you update all the core system packages before proceeding:

       pacman -Syu

   You can browse the [MSYS2 wiki page](https://www.msys2.org/wiki/Home/) for more information.

2. Use the package manager to install the prerequisite packages from the MSYS2 command prompt:

       pacman -S base-devel
       pacman -S git
       pacman -S mingw-w64-x86_64-gnuplot
       pacman -S mingw-w64-x86_64-python-fonttools
       pacman -S mingw-w64-x86_64-python-setuptools
       pacman -S mingw-w64-x86_64-ruby
       pacman -S mingw-w64-x86_64-toolchain
       pacman -S mingw-w64-x86_64-ttfautohint
       pacman -S mingw-w64-x86_64-woff2

   Run the above commands from the MSYS2 shell in the `MSYS` environment. After installing the prerequisite packages, switch to running the MSYS2 shell in the `MINGW64` environment from this point forward.

3. Clone the repository using the `--shallow-exclude` option to exclude the `gh-pages` branch:

       git clone https://github.com/jkillingsworth/jkillingsworth.com.git --shallow-exclude=gh-pages

   The `gh-pages` branch is where the rendered files for the live website are stored.

4. Install the Bundler gem using the following command:

       gem install bundler

5. Install Jekyll and all dependencies using the following command:

       bundle install

6. Verify the latest version of Jekyll is installed using the following command:

       ./bin/jekyll --version

7. Verify that you can build and host the site on your local machine:

       ./bin/jekyll serve

   Give it a few seconds to generate the output files. Navigate to the following URL using your favorite web browser:

   [http://localhost:4000/](http://localhost:4000/)

   You should now be able to see a local copy of the website in your browser.

8. *Optional.* Install [TeX Live](https://www.tug.org/texlive/) if you want to recreate the LaTeX output. Follow the instructions in the documentation. Make sure your `PATH` environment variable includes the directory containing the `latex` and `dvisvgm` executables. Be aware that your MSYS2 environment may not automatically inherit the directories contained in the Windows `PATH` environment variable. I prefer to launch the MSYS2 shell with the following variable set:

       MSYS2_PATH_TYPE=inherit

   This will cause the `PATH` environment variable in your MSYS2 shell to inherit the directories contained in the Windows `PATH` environment variable. You can [consult the wiki](https://www.msys2.org/wiki/MSYS2-introduction/#path) or inspect the [launcher script](https://github.com/msys2/MSYS2-packages/blob/master/filesystem/msys2_shell.cmd) for more insights.

## Usage

* Update the bundler gem to the latest version:

      gem update bundler

* Update all gems in the Gemfile to the latest version:

      bundle update

* Regenerate the Jekyll binstub:

      bundle binstubs jekyll --force

* Build a site:

      ./bin/jekyll build

* Build a site and host it on the local machine:

      ./bin/jekyll serve

  To access the site from another device on the network:

      ./bin/jekyll serve --host 0.0.0.0

  To use incremental rebuild:

      ./bin/jekyll serve --incremental

* Generate an SVG image from a LaTeX input file:

      latex --halt-on-error --interaction=nonstopmode <filename>.tex

  The above generates an intermediate DVI file. To generate the final SVG output:

      dvisvgm <filename>.dvi --no-fonts --exact

* Generate an SVG image from a Gnuplot input file:

      gnuplot <filename>.plt

  The input file must contain the following statement telling the system to use the SVG output terminal:

      set terminal svg
