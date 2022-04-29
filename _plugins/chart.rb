
module Jekyll

    class ChartTag < Liquid::Tag

        def initialize(name, input, tokens)
            super
            @input = input.strip
        end

        def render(context)
            match = @input.match /^fig-(?<figno>\d{2}).*\.svg$/
            figno = match ? match["figno"] : "00"
            post = context.registers[:page]
            path = Assets.source_path(post)
            file = File.join(path, @input)
            name = File.basename(file)

            def capture_px(xml, attribute)
                xml.xpath("/xmlns:svg")[0][attribute]
            end

            if File.exist?(file) then
                xml = open(file, "r") { |f| Nokogiri::XML(f) }
                h = capture_px(xml, "height")
                w = capture_px(xml, "width")
            end

            aspect_ratio = 100 / (w.to_f / h.to_f)

            opening = "<figure class=\"fig-chart\" style=\"padding-top: #{aspect_ratio}%;\">"
            content = "<img src=\".#{post.url}#{name}\" alt=\"Figure #{figno.to_i}\" height=\"#{h}\" width=\"#{w}\" />"
            closing = "</figure>"

            opening + content + closing
        end
    end
end

Liquid::Template.register_tag("chart", Jekyll::ChartTag)
