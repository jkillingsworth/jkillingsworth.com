require "tmpdir"

module Jekyll

    class LatexBlock < Liquid::Block

        def initialize(name, input, tokens)
            super
            @input = input.strip
        end

        def render(context)
            match = @input.match /^fig-(?<figno>\d{2})$/
            figno = match ? match["figno"] : "00"
            latex = super.strip.lines.map{ |s| s.strip.concat("\n") }.join
            fingerprint = Digest::SHA1.hexdigest(latex)[0...8].to_i(16).to_s.rjust(10, "0")
            site = context.registers[:site]
            post = context.registers[:page]
            path = Assets.source_path(post)
            file = File.join(path, "fig-#{figno}-latex-#{fingerprint}.svg")
            name = File.basename(file)

            if !Dir.exist?(path) then
                Dir.mkdir(path)
            end

            def gen_tmpfile(jobname, latex, tempdir)
                Dir.chdir(tempdir) do
                    File.write("#{jobname}.tex", latex)
                    `latex --halt-on-error --interaction=nonstopmode #{jobname}.tex > stdout.log 2>> stderr.log`
                    `dvisvgm #{jobname}.dvi --no-fonts --exact --zoom=1.333333 --prec=6 --verb=3 2>> stderr.log`
                end
            end

            def gen_outfile(jobname, latex, outfile)
                Dir.mktmpdir("#{jobname}-") do |tempdir|
                    gen_tmpfile(jobname, latex, tempdir)
                    File.rename("#{tempdir}/#{jobname}.svg", outfile)
                end
            end

            if !File.exist?(file) then
                puts "LaTeX: ".rjust(20) + "#{file}"
                gen_outfile("latextosvg", latex, file)
                site.static_files << AssetFile.new(site, path, post.url, name)
            end

            def capture_px(xml, attribute)
                value = xml.xpath("/xmlns:svg")[0][attribute]
                value = value.to_s.sub("pt", "").to_f
                value = value * 96 / 72
                value.round(0)
            end

            xml = open(file, "r") { |f| Nokogiri::XML(f) }
            w = capture_px(xml, "width")
            h = capture_px(xml, "height")

            opening = "<figure class=\"fig-latex\">"
            content = "<img width=\"#{w}\" height=\"#{h}\" alt=\"Figure #{figno.to_i}\" src=\".#{post.url}#{name}\" />"
            closing = "</figure>"

            opening + content + closing
        end
    end
end

Liquid::Template.register_tag("latex", Jekyll::LatexBlock)
