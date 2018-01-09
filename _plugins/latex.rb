require "open-uri"

module Jekyll

    class LatexBlock < Liquid::Block

        def initialize(name, input, tokens)
            super
            @input = input.strip
        end

        def render(context)
            figno = @input
            latex = URI.encode(super.strip)
            hash = Digest::SHA1.hexdigest(latex)
            site = context.registers[:site]
            post = context.registers[:page]
            path = File.join($assets_dir, post.path.gsub(/(.*)\/(.*)\.md/, '\2'))
            file = File.join(path, "fig-#{figno}-latex-#{hash[0...8]}.svg")
            name = File.basename(file)

            if !Dir.exist?(path) then
                Dir.mkdir(path)
            end

            if !File.exist?(file) then
                print "Generating #{file}..."
                url = "https://latex.codecogs.com/svg.latex?#{latex}"
                data = open(url, "r") { |f| f.read }
                File.write(file, data)
                site.static_files << AssetFile.new(site, path, post.url, name)
                print "done\n"
            end

            def capture_px(xml, xpath)
                value = xml.xpath(xpath).to_s.sub("pt", "").to_f
                value = value * 4 / 3
                value.round(0)
            end

            xml = open(file, "r") { |f| Nokogiri::XML(f) }
            xml.remove_namespaces!
            w = capture_px(xml, "/svg/@width")
            h = capture_px(xml, "/svg/@height")

            "<img width=\"#{w}\" height=\"#{h}\" alt=\"Figure #{figno.to_i}\" src=\".#{post.url}#{name}\" />"
        end
    end
end

Liquid::Template.register_tag("latex", Jekyll::LatexBlock)
