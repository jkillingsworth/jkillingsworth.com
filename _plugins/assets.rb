
module Jekyll

    class AssetFile < StaticFile

        def path
            File.join(@base, @name)
        end
    end

    class Assets < Generator

        ASSETS_DIR = "_assets"
        INLINE_DIR = "inline"
        STATIC_DIR = "static"

        private_constant :ASSETS_DIR
        private_constant :INLINE_DIR
        private_constant :STATIC_DIR

        def self.source_path(post)
            File.join(ASSETS_DIR, post.path.gsub(/(.*)\/(.*)\.md/, '\2'))
        end

        def self.inline_file(name)
            File.join(ASSETS_DIR, INLINE_DIR, name)
        end

        def generate(site)
            base = File.join(ASSETS_DIR, STATIC_DIR)
            path = STATIC_DIR
            dirs = Pathname(base).each_filename.to_a.count
            Dir[base + "/**/*"].each do |file|
                next if File.directory? file
                name = Pathname(file).each_filename.to_a.drop(dirs).join("/")
                site.static_files << AssetFile.new(site, base, path, name)
            end
            site.posts.docs.each do |post|
                base = Assets.source_path(post)
                path = post.url
                Dir[base + "/*"].each do |file|
                    name = File.basename(file)
                    site.static_files << AssetFile.new(site, base, path, name)
                end
            end
        end
    end
end
