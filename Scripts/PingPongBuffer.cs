using UnityEngine;
internal class PingPongBuffer
{
    internal RenderTexture read { get; private set; }
    internal RenderTexture write { get; private set; }
    internal PingPongBuffer(RenderTexture read, RenderTexture write)
    {
        this.read = read;
        this.write = write;
    }

    internal void Swap()
    {
        var prevRead = read;
        read = write;
        write = prevRead;
    }
}