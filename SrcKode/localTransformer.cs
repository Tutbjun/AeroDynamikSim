using UnityEngine;

public class localTransformer : MonoBehaviour //klasse som er på alt som brugeren kan transformere
{
    void OnMouseOver() {
        if(Input.GetMouseButton(0)){//når man trykker på objektet, så kører den dette
            if(UIHandler.transformVal.magnitude > 0){
                if(UIHandler.translateTriggered){//hvis translate knappen er trykket
                    Vector3 newPos = transform.position;
                    newPos.x += UIHandler.transformVal.x;
                    newPos.y += UIHandler.transformVal.y;
                    newPos.z += UIHandler.transformVal.z;
                    transform.position = newPos;
                }
                if(UIHandler.rotateTriggered){//hvis roter knappen er trykket
                    Vector3 newAngle = transform.rotation.eulerAngles;
                    newAngle.x = UIHandler.transformVal.x;
                    newAngle.y = UIHandler.transformVal.y;
                    newAngle.z = UIHandler.transformVal.z;
                    transform.eulerAngles = newAngle;
                }
                if(UIHandler.scaleTriggered){//hvis scale knappen er trykket
                    Vector3 newScale = transform.localScale;
                    newScale.x *= UIHandler.transformVal.x;
                    newScale.y *= UIHandler.transformVal.y;
                    newScale.z *= UIHandler.transformVal.z;
                    transform.localScale = newScale;
                }
                UIHandler.transformVal = new Vector3();//nulstiller imputfelter
                UIHandler.inputx.Clear();
                UIHandler.inputy.Clear();
                UIHandler.inputz.Clear();
                UIHandler.cursorPosx = -1;
                UIHandler.cursorPosy = -1;
                UIHandler.cursorPosz = -1;
            }
        }
    }
}