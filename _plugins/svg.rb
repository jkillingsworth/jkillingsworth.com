require "open-uri"

module Jekyll
    class SvgTag < Liquid::Tag

        def initialize(name, input, tokens)
            @src = input.strip
        end

        def render(context)
            data = open(@src, "rt") { |f| f.read }
            data.gsub(/\<\?xml.+\?\>|\<\!DOCTYPE.+\>|id\=\".*?\"/, "")
        end
    end
end

Liquid::Template.register_tag("svg", Jekyll::SvgTag)
