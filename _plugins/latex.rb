require "open-uri"

module Jekyll
    class LatexBlock < Liquid::Block

        def render(context)
            latex = URI.encode(super.strip)
            "<img src=\"https://latex.codecogs.com/svg.latex?#{latex}\" alt=\"Equation\" />"
        end
    end
end

Liquid::Template.register_tag("latex", Jekyll::LatexBlock)
