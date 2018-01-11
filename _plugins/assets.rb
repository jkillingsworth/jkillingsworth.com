module Jekyll

    $assets_dir = "_assets"

    class AssetFile < StaticFile

        def path
            File.join(@base, @name)
        end

        def write(dest)
            if path =~ /^.*\/fig-\d{2}-latex-[0-9a-f]{8}\.svg$/ then
                dest_path = destination(dest)

                return false if File.exist?(dest_path) && !modified?
                self.class.mtimes[path] = mtime

                FileUtils.mkdir_p(File.dirname(dest_path))
                FileUtils.rm(dest_path) if File.exist?(dest_path)

                xml = open(path, "r") { |f| Nokogiri::XML(f) }
                xml.xpath("/xmlns:svg")[0]["style"] = "background: white;"
                File.write(dest_path, xml)

                true
            else
                super
            end
        end
    end

    class Assets < Generator

        def generate(site)
            static_dir = "static"
            path = File.join($assets_dir, static_dir)
            Dir[path + "/*"].each do |file|
                name = File.basename(file)
                site.static_files << AssetFile.new(site, path, static_dir, name)
            end
            site.posts.docs.each do |post|
                path = File.join($assets_dir, post.path.gsub(/(.*)\/(.*)\.md/, '\2'))
                Dir[path + "/*"].each do |file|
                    name = File.basename(file)
                    site.static_files << AssetFile.new(site, path, post.url, name)
                end
            end
        end
    end
end
