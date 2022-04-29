require "open3"

module Jekyll

    class LatexBlock < Liquid::Block

        @@latex_upper = '
            \documentclass[varwidth]{standalone}
            \usepackage[charter]{mathdesign}
            \usepackage[scaled=0.75]{roboto-mono}
            \usepackage{mathtools}
            \usepackage{eucal}
            \newcommand{\sscr}[1]{\text{#1}}
            '

        @@latex_upper_1 = '
            \documentclass[varwidth]{standalone}
            \usepackage[charter]{mathdesign}
            \usepackage[scaled=0.75]{roboto-mono}
            \usepackage{mathtools}
            \usepackage{eucal}
            \usepackage{array}
            \usepackage{colortbl}
            \arrayrulecolor[rgb]{0.5,0.5,0.5}
            \renewenvironment{table}[1]
            {
                \setlength{\arraycolsep}{1em}
                \begin{array}{@{\rule{0em}{1.25em}}#1}
            }
            {
                \end{array}
            }
            \renewenvironment{matrix}[1]
            {
                \mathopen{}\mathclose\bgroup\left[
                \begin{array}{#1}
            }
            {
                \end{array}
                \aftergroup\egroup\right]
            }
            \renewcommand{\brace}[4]
            {
                \if#1*
                    \mathopen{}\mathclose\bgroup\left#2
                    #4
                    \aftergroup\egroup\right#3
                \else
                    \ifcase#1#2\or\bigl#2\or\Bigl#2\or\biggl#2\or\Biggl#2\else#2\fi
                    #4
                    \ifcase#1#3\or\bigr#3\or\Bigr#3\or\biggr#3\or\Biggr#3\else#3\fi
                \fi
            }
            \newcommand{\0}{\mspace{0.5mu}}
            \newcommand{\1}{\mspace{1.0mu}}
            \newcommand{\2}{\mspace{1.5mu}}
            \newcommand{\tsum}{\mathop{\textstyle\sum}}
            \newcommand{\dderiv}{d\mspace{-1.5mu}}
            \newcommand{\pderiv}{\partial\mspace{-2.0mu}}
            \newcommand{\sscr}[1]{\text{#1}}
            '

        def process_latex(items)
            items.map { |s| s.strip.lines }.flatten.map { |s| s.strip.concat("\n") }.join
        end

        def initialize(name, input, tokens)
            super
            @input = input.strip
        end

        def render(context)
            match = @input.match /^((?<preamble>\d{1}) )?fig-(?<figno>\d{2})$/
            figno = match ? match["figno"] : "00"
            latex_inner = process_latex([super])
            fingerprint = Digest::SHA1.hexdigest(latex_inner)[0...8].to_i(16).to_s.rjust(10, "0")
            site = context.registers[:site]
            post = context.registers[:page]
            path = Assets.source_path(post)
            file = File.join(path, "fig-#{figno}-latex-#{fingerprint}.svg")
            name = File.basename(file)
            opts = site.config["latextosvg"]

            if !Dir.exist?(path) then
                Dir.mkdir(path)
            end

            def print_e(message)
                color_error = "\e[1;31m"
                color_reset = "\e[0m"
                STDERR.print(color_error, message, color_reset)
            end

            def gen_outfile(latex, opts, outfile)
                fonts = opts["fonts"]
                stdout, stderr, status = Open3.capture3("bash ./bin/latextosvg.sh -f #{fonts}", :stdin_data => latex)
                print_e stderr
                unless !status.success? then
                    File.write(outfile, stdout)
                end
            end

            if !File.exist?(file) then
                puts "LaTeX: ".rjust(20) + "#{file}"
                preamble = match["preamble"]
                case preamble
                when nil
                    latex_upper = @@latex_upper
                when "1"
                    latex_upper = @@latex_upper_1
                else
                    raise "Unknown preamble option '#{preamble}'"
                end
                latex = process_latex([latex_upper, latex_inner])
                gen_outfile(latex, opts, file)
                site.static_files << AssetFile.new(site, path, post.url, name)
            end

            def capture_px(xml, attribute)
                value = xml.xpath("/xmlns:svg")[0][attribute]
                value = value.to_s.sub("pt", "").to_f
                value = value * 96 / 72
                value.round(0)
            end

            xml = open(file, "r") { |f| Nokogiri::XML(f) }
            h = capture_px(xml, "height")
            w = capture_px(xml, "width")

            opening = "<figure class=\"fig-latex\">"
            content = "<img src=\".#{post.url}#{name}\" alt=\"Figure #{figno.to_i}\" height=\"#{h}\" width=\"#{w}\" />"
            closing = "</figure>"

            opening + content + closing
        end
    end
end

Liquid::Template.register_tag("latex", Jekyll::LatexBlock)
