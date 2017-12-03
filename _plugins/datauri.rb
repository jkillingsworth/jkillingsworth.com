require "base64"
require "mimemagic"
require "open-uri"

module Jekyll
    class DataUriTag < Liquid::Tag

        def initialize(name, input, tokens)
            @src = input.strip
        end

        def render(context)
            data = open(@src, "rb") { |f| data = f.read }
            type = MimeMagic.by_magic(data)
            encoded = Base64.strict_encode64(data)
            "data:#{type};base64,#{encoded}"
        end
    end
end

Liquid::Template.register_tag("datauri", Jekyll::DataUriTag)
