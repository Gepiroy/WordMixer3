using Microsoft.SqlServer.Server;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMixer3 {
    public static class Audio {
        static AudioFileReader afr; //Через него гораздо удобнее управлять громкостью, поэтому не using, а загрузка в статик.

        static WaveFormat format;
        public static int bytesPerFrame;
        static int step;//минимальное число байт для чтения. По-сути это bytesPerSample: учитывает и число каналов, и число байт на каждый.
        public static int totalFrames;
        public static bool isLoaded=false;

        public static void loadWav(string path) {
            afr = new AudioFileReader(path);

            format = afr.WaveFormat;
            step = format.BitsPerSample / 8;
            bytesPerFrame = format.SampleRate * step / App.fps;
            bytesPerFrame -= bytesPerFrame % step;//Если не сделать, будет ад.
            totalFrames = (int)(afr.Length / bytesPerFrame);
            isLoaded = true;
        }
        public static byte[] getPiece(int frame, int length) { //Достаёт из потока отрывок, приглушая начало и конец.
            int lengthInBytes = bytesPerFrame * length;//Длина отрезка, кой добавляем, в байтах.
            afr.Position = frame * bytesPerFrame;
            byte[] toHear = new byte[lengthInBytes];//Это по-сути ret.

            int offset = 0;//Это позиция, в какой конкретный индекс вписывать добавляемое.
            int eatPiece = bytesPerFrame / 4 - (bytesPerFrame / 4) % step; //Это число байтов, которые нужно приглашуть в начале и конце. 1/4 от кадра.

            for (int i = 0; i < eatPiece; i += step) {//Добавляем кусок с нарастанием.
                afr.Volume = 1.0f * i / eatPiece;
                afr.Read(toHear, offset, step);
                offset += step;
            }
            afr.Volume = 1;
            afr.Read(toHear, offset, lengthInBytes - eatPiece * 2); //Считываем середину куска с норм громкостью
            offset = lengthInBytes - eatPiece;//Ставим курсор чётко на концовку
            for (int i = offset; i < lengthInBytes; i += step) {
                afr.Volume = 1.0f * (lengthInBytes - i) / eatPiece;
                afr.Read(toHear, offset, step);
                offset += step;
            }
            return toHear;
        }
        public static byte[] genVoid(int length) { //Тупо даёт массив с нулями в том количестве, в каком надо по фреймам.
            int lengthInBytes = bytesPerFrame * length;//Длина отрезка, кой добавляем, в байтах.
            return new byte[lengthInBytes];
        }
        public static void play(byte[] bytes) {
            IWaveProvider provider = new RawSourceWaveStream(
                         new MemoryStream(bytes), format);
            var wo = new WaveOutEvent();
            wo.Init(provider);
            wo.Play();
        }
        public static void render(string path, byte[] bytes) {
            using (WaveFileWriter writer = new WaveFileWriter(path, format)) {
                writer.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
