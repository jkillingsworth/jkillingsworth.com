require "nokogiri"
require "text/hyphen"

module Jekyll

    module HyphenationFilter

        @@hyphenator = Text::Hyphen.new(language: "en_us", :left => 2, :right => 2)

        def hyphenate_and_fixup(content)
            content = hyphenate(content)
            content = content.gsub(/-\u00ad/, "-")
            content = content.gsub(/&amp;am(\u00ad?)p;/, "&amp;")
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
                regex = /#{Regexp.escape(word)}(?!\z)/
                hyphenated_word = @@hyphenator.visualize(word, "\u00ad")
                text.gsub!(regex, hyphenated_word)
            end
            text
        end
    end
end

Liquid::Template.register_filter(Jekyll::HyphenationFilter)
