import { Quaternion } from 'three';







export class MixamoConversion {

    

    convertToMixamoRotation = function(boneName,  boneQuat, rotationData) {
        
        
        let inputQuat;
        switch (boneName) {
        case 'LeftUpLeg':
        case 'LeftLeg':
        case 'LeftFoot':
            inputQuat = new Quaternion(
                rotationData[3],
                -rotationData[1],
                -rotationData[2],
                rotationData[0]
            );
            break;
        case 'RightUpLeg':
        case 'RightLeg':
        case 'RightFoot':
            inputQuat = new Quaternion(
                rotationData[3],
                rotationData[1],
                rotationData[2],
                rotationData[0]
            );
            break;
        case 'Spine':
        case 'Spine1':
        case 'Spine2':
        case 'Neck':
        case 'Head':
            inputQuat = new Quaternion(
                -rotationData[3],
                rotationData[1],
                -rotationData[2],
                rotationData[0]
            );
            break;
        case 'Hips':
            inputQuat = new Quaternion(
                rotationData[1],
                rotationData[2],
                rotationData[3],
                rotationData[0]
            );
            break;
        case 'RightShoulder':
        case 'RightArm':
        case 'RightForeArm':
        case 'RightHand':
            inputQuat = new Quaternion(
                rotationData[2],
                -rotationData[1],
                rotationData[3],
                rotationData[0]
            );
            // .multiply(new Quaternion(0, 0, 0, 1));
            break;
        case 'LeftShoulder':
        case 'LeftArm':
        case 'LeftForeArm':
        case 'LeftHand':
            inputQuat = new Quaternion(
                rotationData[2],
                rotationData[1],
                -rotationData[3],
                rotationData[0]
            );
            // .multiply(new Quaternion(0, 0, 0, 1));
            break;
        default:
            inputQuat = new Quaternion(
                rotationData[3],
                rotationData[1],
                rotationData[2],
                rotationData[0]
            );
            break;
        }
        const quat = new Quaternion()
            .multiply(boneQuat.clone())
            .multiply(inputQuat.clone());
    
        return quat;
    };





}