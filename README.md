## Overview

This is the source code for my personal website.

## Toolchain

The bulid the site, you need to install [Ruby](https://www.ruby-lang.org/) and use the [Jekyll](https://jekyllrb.com/) static site generator.

#### Installation (Windows)

Follow the instructions for installing [Jekyll on Windows](https://jekyllrb.com/docs/windows/#installation-via-rubyinstaller):

1. Download and run [RubyInstaller](https://rubyinstaller.org/downloads/) for Windows.

   *Note:* Avoid installing in a folder name that contains spaces (e.g. Program Files). Having spaces in the shebang interpreter path causes problems, even if you put quotes around it. See here for more details:

   https://lists.gnu.org/archive/html/bug-bash/2008-05/msg00051.html

   *Optional*: Check the box to use UTF-8 as the default external encoding. See section titled [Encoding](https://jekyllrb.com/docs/windows/#encoding) in the instructions.

2. Install the MSYS2 components when prompted. Use the default options.

3. Install Jekyll and Bundler using the following command:

       gem install jekyll bundler

   Use the following command to verify the latest version of Jekyll is installed:

       jekyll -v

#### Usage

* Install gems needed for an existing site:

      bundle install

* Install gems needed for an existing site into a local path:

      bundle install --path=<name>

* Update gems to the latest version:

      bundle update

* Execute command in the context of a local path installation:

      bundle exec <command>

* Create a new blank site:

      jekyll new <name> --blank

* Build a site:

      jekyll build

* Build a site and host it on the local machine:

      jekyll serve

* Build and host a site with incremental rebuild:

      jekyll serve --incremental
