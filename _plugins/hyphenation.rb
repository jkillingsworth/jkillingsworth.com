require "nokogiri"
require "text/hyphen"

module Jekyll

    module HyphenationFilter

        @@hyphenator = Text::Hyphen.new(language: "en_us", :left => 1, :right => 1)

        def process_fixups(content)
            content = content.gsub(/&amp;am(\u00ad?)p;/, "&amp;")
            content = content.gsub(/\u002d\u00ad/, "\u002d")
            content = content.gsub(/\u2013\u00ad/, "\u2013")
            content = content.gsub(/\u2014\u00ad/, "\u2014")
            content
        end

        def hyphenate_text(content)
            content.split.each do |word|
                content.sub!(word, @@hyphenator.visualize(word, "\u00ad"))
            end
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
            process_fixups(fragment.to_s)
        end
    end
end

Liquid::Template.register_filter(Jekyll::HyphenationFilter)
