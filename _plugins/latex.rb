module Jekyll

    class LatexBlock < Liquid::Block

        def render(context)

            latex = URI.encode(super.strip)
            hash = Digest::SHA1.hexdigest(latex)
            site = context.registers[:site]
            post = context.registers[:page]
            path = File.join("_assets", post.path.gsub(/(.*)\/(.*)\.md/, '\2'))
            file = File.join(path, "#{hash}.svg")
            name = File.basename(file)

            if !Dir.exist?(path) then
                Dir.mkdir(path)
            end

            if !File.exist?(file) then
                print "Generating #{file}..."
                url = "https://latex.codecogs.com/svg.latex?#{latex}"
                data = open(url, "r") { |f| f.read }
                File.write(file, data)
                site.static_files << AssetFile.new(site, path, post.url, name)
                print "done\n"
            end

            "<img src=\".#{post.url}/#{name}\" alt=\"Equation #{hash[0...7]}\" />"
        end
    end
end

Liquid::Template.register_tag("latex", Jekyll::LatexBlock)
