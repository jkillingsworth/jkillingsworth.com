
module Jekyll

    class TableBlock < Liquid::Block

        def initialize(name, input, tokens)
            super
            @input = input.strip
        end

        def render(context)
            site = context.registers[:site]
            conv = site.find_converter_instance(Jekyll::Converters::Markdown)
            text = super.strip

            opening = "<figure class=\"fig-table\">"
            content = conv.convert(text)
            closing = "</figure>"

            opening + content + closing
        end
    end
end

Liquid::Template.register_tag("table", Jekyll::TableBlock)
