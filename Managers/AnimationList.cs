using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace WpfOpenTK.Managers
{
    public enum AnimationStep
    {
        Rotations,
        Translations
    };

    public class AnimationList : IEnumerable
    {
        ArrayList key_points = new ArrayList();

        public void Push(AnimationStep anim_step, double xValue, double yValue, double zValue)
        {
            var values = new ArrayList();
            values.Add(xValue);
            values.Add(yValue);
            values.Add(zValue);

            KeyValuePair<AnimationStep, ArrayList> keyValue_temp =
                new KeyValuePair<AnimationStep, ArrayList>(anim_step, values);
            key_points.Add(keyValue_temp);
        }

        public int Count()
        {
            return key_points.Count;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            foreach (var item in key_points)
            {
                yield return item;
            }

        }

    }
}
