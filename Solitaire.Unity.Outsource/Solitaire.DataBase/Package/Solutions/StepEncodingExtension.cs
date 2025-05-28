using System.Collections.Generic;
using System.IO;

namespace Solitaire.DataBase.Solutions
{
    public static class StepEncodingExtension
    {
        public static BinaryWriter Write(this BinaryWriter writer, IEnumerable<Step> steps)
        {
            foreach (var i in steps) {
                writer.Write(i);
            }
            return writer;
        }
        /// <summary>
        /// пишет один шаг в поток
        /// </summary>
        /// <param name="writer">врайтер</param>
        /// <param name="step">один шаг в игре</param>
        public static BinaryWriter Write(this BinaryWriter writer, Step step)
        {
            writer.Write((byte)step.StepType);
            switch (step.StepType) {
                case StepTypes.MoveCards:
                    writer.Write((byte)(step.Arg0 | step.Arg2 << 4));
                    writer.Write((byte)step.Arg1);
                    return writer;
                case StepTypes.MoveFromWaste:
                    writer.Write((byte)step.Arg0);
                    return writer;
                case StepTypes.MoveToFoundation:
                    return writer;
                case StepTypes.MoveFromWasteToFoundation:
                    return writer;
                case StepTypes.DrawFromStock:
                    return writer;
            }

            writer.Write((byte)step.Arg0);
            writer.Write((byte)step.Arg1);
            writer.Write((byte)step.Arg2);
            return writer;
        }
        /// <summary>
        /// читает шаг из потока
        /// </summary>
        /// <param name="reader">читатель потока</param>
        public static Step ReadStep(this BinaryReader reader)
        {
            var step = new Step();
            step.StepType = (StepTypes)reader.ReadByte();

            switch (step.StepType) {
                case StepTypes.MoveCards:
                    var data = reader.ReadByte();
                    step.Arg0 = 0x0f & data;
                    step.Arg1 = reader.ReadByte();
                    step.Arg2 = (0xf0 & data) >> 4;
                    return step;
                case StepTypes.MoveToFoundation:
                    return step;
                case StepTypes.MoveFromWaste:
                    step.Arg0 = reader.ReadByte();
                    return step;
                case StepTypes.MoveFromWasteToFoundation:
                    return step;
                case StepTypes.DrawFromStock:
                    return step;
            }

            step.Arg0 = reader.ReadByte();
            step.Arg1 = reader.ReadByte();
            step.Arg2 = reader.ReadByte();

            return step;
        }
        public static Step ReadStepOld(this BinaryReader reader)
        {
            var step = new Step();
            step.StepType = (StepTypes)reader.ReadByte();

            switch (step.StepType) {
                case StepTypes.MoveCards:
                    var data = reader.ReadByte();
                    step.Arg0 = 0x0f & data;
                    step.Arg1 = reader.ReadByte();
                    step.Arg2 = (0xf0 & data) >> 4;
                    return step;
                case StepTypes.MoveToFoundation:
                    step.Arg0 = reader.ReadByte();
                    return step;
                case StepTypes.MoveFromWaste:
                    step.Arg0 = reader.ReadByte();
                    return step;
                case StepTypes.MoveFromWasteToFoundation:
                    return step;
                case StepTypes.DrawFromStock:
                    return step;
            }

            step.Arg0 = reader.ReadByte();
            step.Arg1 = reader.ReadByte();
            step.Arg2 = reader.ReadByte();

            return step;
        }
        /// <summary>
        /// читает все шаги до конца потока
        /// </summary>
        /// <param name="reader">читатель</param>
        /// <returns></returns>
        public static List<Step> ReadStepsToEnd(this BinaryReader reader)
        {
            var list = new List<Step>();
            while (reader.BaseStream.Position != reader.BaseStream.Length) {
                list.Add(reader.ReadStep());
            }
            return list;
        }
        public static List<Step> ReadStepsToEndOld(this BinaryReader reader)
        {
            var list = new List<Step>();
            while (reader.BaseStream.Position != reader.BaseStream.Length) {
                list.Add(reader.ReadStepOld());
            }
            return list;
        }
    }
}