﻿using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class CompressedDestination : IDestination
    {
        private readonly IDestination another;

        public CompressedDestination(IDestination another)
        {
            Guard.NotNull(() => another, another);
            this.another = another;
        }

        public bool IsDownloaded(VideoHeader header)
        {
            Guard.NotNull(() => header, header);
            return another.IsDownloaded(ConstructHeader(header));
        }

        public async Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            using (var memory = new MemoryStream())
                using (var zipStream = new GZipOutputStream(memory))
                {
                    source.Position = 0;
                    source.CopyTo(zipStream);
                    zipStream.Flush();
                    zipStream.Finish();
                    memory.Position = 0;

                    await another.Transfer(
                                     ConstructHeader(header),
                                     memory)
                                 .ConfigureAwait(false);
                }
        }

        private static VideoHeader ConstructHeader(VideoHeader header)
        {
            return new VideoHeader(header.Camera, Path.ChangeExtension(header.FileName, "zip"));
        }
    }
}