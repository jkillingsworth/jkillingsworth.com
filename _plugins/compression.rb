require "htmlcompressor"

module Jekyll

    options = { :remove_intertag_spaces => true }
    compressor = HtmlCompressor::Compressor.new(options)
    compression_enabled = true

    Jekyll::Hooks.register(:pages, :post_render) do |page|
        if (compression_enabled && page.html?) then
            page.output = compressor.compress(page.output)
        end
    end

    Jekyll::Hooks.register(:posts, :post_render) do |post|
        if (compression_enabled) then
            post.output = compressor.compress(post.output)
        end
    end
end
