require "open-uri"

module Jekyll

    class InlineSvgTag < Liquid::Tag

        def initialize(name, input, tokens)
            super
            @input = input.strip
        end

        def render(context)
            name = @input
            file = Assets.inline_file(name)
            data = open(file, "r") { |f| f.read }
            data.gsub(/\<\?xml.+\?\>|id\=\".*?\"/, "")
        end
    end
end

Liquid::Template.register_tag("inline_svg", Jekyll::InlineSvgTag)
