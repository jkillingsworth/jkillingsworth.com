require "nokogiri"
require "text/hyphen"

module Jekyll

    module HyphenationFilter

        @@hyphenator = Text::Hyphen.new(language: "en_us", :left => 2, :right => 2)

        def hyphenate_and_fixup(content)
            content = hyphenate(content)
            content = content.gsub(/&amp;am(\u00ad?)p;/, "&amp;")
            content = content.gsub(/\u002d\u00ad/, "\u002d")
            content = content.gsub(/\u2013\u00ad/, "\u2013")
            content = content.gsub(/\u2014\u00ad/, "\u2014")
            content
        end

        def hyphenate(content)
            selector = @context.registers[:site].config["hyphenation"]["selector"]
            fragment = Nokogiri::HTML::DocumentFragment.parse(content)
            fragment.css(selector).each do |element|
                element.traverse do |node|
                    node.content = hyphenate_text(node.to_s) if node.text?
                end
            end
            fragment.to_s
        end

        def hyphenate_text(text)
            text.split.each do |word|
                text.sub!(word, @@hyphenator.visualize(word, "\u00ad"))
            end
            text
        end
    end
end

Liquid::Template.register_filter(Jekyll::HyphenationFilter)
