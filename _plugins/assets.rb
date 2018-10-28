require "open-uri"

module Jekyll

    class AssetFile < StaticFile

        def path
            File.join(@base, @name)
        end

        def write(dest)
            if path =~ /^.*\/fig-\d{2}-latex-[0-9]{10}\.svg$/ then
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
            output_path = File.join("/", STATIC_DIR)
            path = File.join(ASSETS_DIR, STATIC_DIR)
            Dir[path + "/*"].each do |file|
                name = File.basename(file)
                site.static_files << AssetFile.new(site, path, output_path, name)
            end
            site.posts.docs.each do |post|
                path = Assets.source_path(post)
                Dir[path + "/*"].each do |file|
                    name = File.basename(file)
                    site.static_files << AssetFile.new(site, path, post.url, name)
                end
            end
        end
    end
end
