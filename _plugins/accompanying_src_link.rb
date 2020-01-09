
module Jekyll

    class AccompanyingSrcLinkTag < Liquid::Tag

        def render(context)
            post = context.registers[:page]
            link = "https://github.com/jkillingsworth/jkillingsworth.com/tree/master/_src/"
            link = link + post.id.gsub("/", "-").slice(1..-1)

            opening = "<p class=\"nojustify\"><a href=\"#{link}\">"
            content = "Accompanying source code is available on GitHub."
            closing = "</a></p>"

            opening + content + closing
        end
    end
end

Liquid::Template.register_tag("accompanying_src_link", Jekyll::AccompanyingSrcLinkTag)
