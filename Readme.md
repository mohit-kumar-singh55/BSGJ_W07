# 衝突がないように
- mainブランチは変更しないように一度ブランチを切ってから編集をしてプルリクエストを作成するようにお願いします。
- プルリクエストのレビュワーには僕をアサインしてください。
- 各々は自分の名前が付けたシーンを作成し、タスクとテストを行ってください
- タスクが終わったら、その部分をメインシーンに移動してください


# To use Vosk for Speech Detection
- Download Japanese Language Model from [this link](https://alphacephei.com/vosk/models/vosk-model-ja-0.22.zip)
- Extract it. and make sure the folder name should be "vosk-model-ja-0.22"
- Place the whole folder under "Assets/StreamingAssets/"

# To use Mediapipe for Hand Gesture Detection
- Download .tgz file from [this link](https://github.com/homuler/MediaPipeUnityPlugin/releases/download/v0.16.3/com.github.homuler.mediapipe-0.16.3.tgz)
- Don't Extract it.
- Place the whole folder under "Assets/UnUploadables/"
- Then open the Unity Editor

# To run this game after build, requires "vc_redist.x64.exe" installation in that PC
- Download this file from [here](https://aka.ms/vc14/vc_redist.x64.exe)
- run and install it and then restart before runnig the game