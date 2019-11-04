# Aspect Ratio Adapter

This is a simple solution to support two different UI layouts where the same object has a different RectTransform for panoramic layout and tablet layout.

### How to use it
Simple add the script "AspectRatioAdapter" to the object and directly edit the RectTransform. The script will save it as panoramic or tablet according the current GameView resolution. To edit the other layout simply change the resolution on the GameView.

The other transform can be seen on the component under the foldout.

The checkbox on the script determines if the transform swap should be done also while playing if a resolution change is detected. Otherwise only on awake the corresponding transform will be applied.