module Jekyll

    class AssetFile < StaticFile
        def path
            File.join(@base, @name)
        end
    end

    class Assets < Generator

        def generate(site)
            site.posts.docs.each do |post|
                path = File.join("_assets", post.path.gsub(/(.*)\/(.*)\.md/, '\2'))
                Dir[path + "/*"].each do |file|
                    name = File.basename(file)
                    site.static_files << AssetFile.new(site, path, post.url, name)
                end
            end
        end
    end
end
