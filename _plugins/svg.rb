require "open-uri"

module Jekyll

    class SvgTag < Liquid::Tag

        def initialize(name, input, tokens)
            super
            @input = input.strip
        end

        def render(context)
            file = File.join($assets_dir, @input)
            data = open(file, "r") { |f| f.read }
            data.gsub(/\<\?xml.+\?\>|id\=\".*?\"/, "")
        end
    end
end

Liquid::Template.register_tag("svg", Jekyll::SvgTag)
