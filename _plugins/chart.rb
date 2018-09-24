require "open-uri"

module Jekyll

    class ChartTag < Liquid::Tag

        def initialize(name, input, tokens)
            super
            @input = input.strip
        end

        def render(context)
            name = @input
            post = context.registers[:page]
            path = Assets.source_path(post)
            file = File.join(path, name)

            match = name.match /^fig-(?<figno>\d{2}).*\.svg$/
            figno = match ? match["figno"] : "00"

            def capture_px(xml, attribute)
                xml.xpath("/xmlns:svg")[0][attribute]
            end

            if File.exist?(file) then
                xml = open(file, "r") { |f| Nokogiri::XML(f) }
                w = capture_px(xml, "width")
                h = capture_px(xml, "height")
            end

            "<img width=\"#{w}\" height=\"#{h}\" alt=\"Figure #{figno.to_i}\" src=\".#{post.url}#{name}\" />"
        end
    end
end

Liquid::Template.register_tag("chart", Jekyll::ChartTag)
