module Jekyll

    $assets_dir = "_assets"

    class AssetFile < StaticFile

        def path
            File.join(@base, @name)
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
