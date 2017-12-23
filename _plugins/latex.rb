require "open-uri"

module Jekyll
    class LatexBlock < Liquid::Block

        def render(context)

            latex = URI.encode(super.strip)
            hash = Digest::SHA1.hexdigest(latex)
            path = "./assets/latex/#{hash}.svg"

            if !File.exist?(path) then
                print "Generating #{path} ..."
                url = "https://latex.codecogs.com/svg.latex?#{latex}"
                data = open(url, "r") { |f| f.read }
                open(path, "w") { |f| f.write(data) }
                print "done.\n"
            end

            "<img src=\"#{path}\" alt=\"Equation #{hash[0...7]}\" />"
        end
    end
end

Liquid::Template.register_tag("latex", Jekyll::LatexBlock)
