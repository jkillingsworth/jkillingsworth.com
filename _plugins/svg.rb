require "open-uri"

module Jekyll
    class SvgTag < Liquid::Tag

        def initialize(name, input, tokens)
            @src = input.strip
        end

        def render(context)
            text = open(@src, "rt") { |f| data = f.read }
            text.gsub(/\<\?xml.+\?\>|\<\!DOCTYPE.+\>|id\=\".*?\"/, "")
        end
    end
end

Liquid::Template.register_tag("svg", Jekyll::SvgTag)
