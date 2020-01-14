## Overview

This is the source code for my personal website.

## Toolchain

The build the site, you need to install [Ruby](https://www.ruby-lang.org/) and use the [Jekyll](https://jekyllrb.com/) static site generator. Optionally, you may need to install additional tools if you want to recreate the [LaTeX](https://www.latex-project.org/) output or compile the source code used to generate the charts and data that appear in some of the blog posts.

#### Installation

The following instructions are designed for installing the required tools on a Windows system. However, all of these tools are all available for other platforms as well.

1. Download and run [RubyInstaller](https://rubyinstaller.org/downloads/) for Windows. See the instructions for installing [Jekyll on Windows](https://jekyllrb.com/docs/installation/windows/#installation-via-rubyinstaller).

   *Note:* Avoid installing in a folder name that contains spaces (e.g. Program Files). Having spaces in the shebang interpreter path causes problems, even if you put quotes around it. See here for more details:

   https://lists.gnu.org/archive/html/bug-bash/2008-05/msg00051.html

   *Optional:* Check the box to use UTF-8 as the default external encoding. See section titled [Encoding](https://jekyllrb.com/docs/installation/windows/#encoding) in the instructions.

2. Install the MSYS2 components when prompted. Use the default options.

3. Install the Bundler gem using the following command:

       gem install bundler

4. Install Jekyll and all dependencies using the following command:

       bundle install

5. Verify the latest version of Jekyll is installed using the following command:

       jekyll -v

6. *Optional.* Install [TeX Live](https://www.tug.org/texlive/) if you want to recreate the LaTeX output.

7. *Optional.* Install [Gnuplot](http://www.gnuplot.info/) if you want to recreate the charts. When running the installer, check the option to add the application directory to your PATH environment variable.

   *Note:* You will also need to compile the [F#](https://fsharp.org/) source code that generates the data displayed in the charts.

#### Usage

* Install gems needed for an existing site:

      bundle install

* Update gems to the latest version:

      bundle update

* Build a site:

      jekyll build

* Build a site and host it on the local machine:

      jekyll serve

  To access the site from another device:

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
