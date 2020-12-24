require "jekyll/hyphenate_filter"

module Jekyll

    module HyphenationFilter

        def hyphenate_and_fixup(content)
            content = hyphenate(content)
            content = content.gsub(/-\u00ad/, "-")
            content = content.gsub(/&amp;am(\u00ad?)p;/, "&amp;")
            content
        end
    end
end

Liquid::Template.register_filter(Jekyll::HyphenationFilter)
