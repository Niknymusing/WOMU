## Pose

Pose takes a 2D vector illustration and animates its containing curves in real-time based on the recognition result from PoseNet. It borrows the idea of skeleton-based animation from computer graphics and applies it to vector characters.

This is running in the browser in realtime using [TensorFlow.js](https://www.tensorflow.org/js).

In skeletal animation a character is represented in two parts:
1. a surface used to draw the character, and 
2. a hierarchical set of interconnected bones used to animate the surface. 

In Pose, the surface is defined by the 2D vector paths in the input SVG files. For the bone structure, Pose provides a predefined rig (bone hierarchy) representation, designed based on the keypoints from PoseNet. This bone structure’s initial pose is specified in the input SVG file, along with the character illustration, while the real time bone positions are updated by the recognition result from ML models.

### Build And Run

Install dependencies and prepare the build directory:

```sh
yarn
```

To watch files for changes, and launch a dev server:

```sh
yarn watch
```

### Platform support

Demos are supported on Desktop Chrome and iOS Safari.

It should also run on Chrome on Android and potentially more Android mobile browsers though support has not been tested yet.

### Animate own avatar

1. Download the [sample skeleton SVG here](/resources/samples/skeleton.svg).
2. Create a new file in your vector graphics editor of choice. Copy the group named ‘skeleton’ from the above file into your working file. Note: 
    * Do not add, remove or rename the joints (circles) in this group. Pose Animator relies on these named paths to read the skeleton’s initial position. Missing joints will cause errors.
    * However you can move the joints around to embed them into your illustration. See step 4.
3. Create a new group and name it ‘illustration’, next to the ‘skeleton’ group. This is the group where you can put all the paths for your illustration.
    * Flatten all subgroups so that ‘illustration’ only contains path elements.
    * Composite paths are not supported at the moment.
    * The working file structure should look like this:
    ```
        [Layer 1]
        |---- skeleton
        |---- illustration
              |---- path 1
              |---- path 2
              |---- path 3
    ```
4. Embed the sample skeleton in ‘skeleton’ group into your illustration by moving the joints around.
5. Export the file as an SVG file.

