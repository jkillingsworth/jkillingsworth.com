## Overview

This is the source repository for my personal website.

## Toolchain

The build the site, you'll need to install [Ruby](https://www.ruby-lang.org/) and use the [Jekyll](https://jekyllrb.com/) static site generator. Optionally, you may need to install some additional tools if you want to recreate the [LaTeX](https://www.latex-project.org/) output or compile the source code used to generate the charts and data that appear in some of the blog posts.

#### Installation

The following instructions are designed for installing the required toolchain on a Windows system. However, all of these tools are all available on most Unix-like platforms as well.

1. Download and run the [MSYS2](https://www.msys2.org/) installer. Follow the installation instructions documented on the website. Make sure you update all the core system packages before proceeding:

       pacman -Syu

   You can browse the [MSYS2 wiki page](https://github.com/msys2/msys2/wiki) for more information.

2. Use the package manager to install the prerequisite packages from the MSYS2 command prompt:

       pacman -S make
       pacman -S mingw-w64-x86_64-toolchain
       pacman -S mingw-w64-x86_64-ruby

   *Note:* The build tools are necessary because some of the Ruby gems installed in the following steps contain native components that are built from source upon installation.

   *Note:* You should run the MSYS2 shell in the `MINGW64` environment from this point forward.

3. Install the Bundler gem using the following command:

       gem install bundler

4. Install Jekyll and all dependencies using the following command:

       bundle install

5. Verify the latest version of Jekyll is installed using the following command:

       jekyll --version

6. Verify that you can build and host the site on your local machine:

       jekyll serve

   Give it a few seconds to generate the output files. Navigate to the following URL using your favorite web browser:

   [http://localhost:4000/](http://localhost:4000/)

   You should now be able to see a local copy of the website in your browser.

7. *Optional.* Install the [Git](https://git-scm.com/) package of you want to use Git from within the MSYS2 environment:

       pacman -S git

8. *Optional.* Install [TeX Live](https://www.tug.org/texlive/) if you want to recreate the LaTeX output. Follow the instructions in the documentation. Make sure your `PATH` environment variable includes the directory containing the `latex` and `dvisvgm` executables. Be aware that your MSYS2 environment may not automatically inherit the directories contained in the Windows `PATH` environment variable. I prefer to launch the MSYS2 shell with the following variable set:

       MSYS2_PATH_TYPE=inherit

   This will cause the `PATH` environment variable in your MSYS2 shell to inherit the directories contained in the Windows `PATH` environment variable. You can [consult the wiki](https://github.com/msys2/msys2/wiki/MSYS2-introduction#path) or inspect the [launcher script](https://github.com/msys2/MSYS2-packages/blob/master/filesystem/msys2_shell.cmd) for more insights.

9. *Optional.* Install [Gnuplot](http://www.gnuplot.info/) if you want to recreate the charts. When running the installer, check the option to add the application directory to your `PATH` environment variable. You will also need a build environment capable of building and running the [F#](https://fsharp.org/) code that generates the data displayed in the charts. You can use [Visual Studio](https://visualstudio.microsoft.com/) for this.

#### Usage

* Update the bundler gem to the latest version:

      gem update bundler

* Update all gems in the Gemfile to the latest version:

      bundle update

* Build a site:

      jekyll build

* Build a site and host it on the local machine:

      jekyll serve

  To access the site from another device on the network:

      jekyll serve --host 0.0.0.0

  To use incremental rebuild:

      jekyll serve --incremental

* Generate an SVG image from a LaTeX input file:

      latex --halt-on-error --interaction=nonstopmode <filename>.tex

  The above generates an intermediate DVI file. To generate the final SVG output:

      dvisvgm <filename>.dvi --no-fonts --exact

* Generate an SVG image from a Gnuplot input file:

      gnuplot <filename>.plt

  The input file must contain the following statement telling the system to use the SVG output terminal:

      set terminal svg
