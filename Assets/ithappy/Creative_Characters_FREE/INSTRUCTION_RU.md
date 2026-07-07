# Creative Characters FREE - инструкция по созданию и настройке персонажа

Инструкция подготовлена для проекта на Unity 6000.5.2f1 и текущей локальной структуры:

`Assets/ithappy/Creative_Characters_FREE`

Asset Store:

https://assetstore.unity.com/packages/3d/characters/humanoids/creative-characters-free-animated-pack-304841

На странице Asset Store на 2026-07-05 указаны: пакет бесплатный, publisher `ithappy`, latest version `2.4`, latest release date `Jun 4, 2026`, original Unity version `2022.3.22`, совместимость с Unity `6000.0.28f1`, Built-in/URP/HDRP. В локальных `.meta` у части файлов встречается `packageVersion: 2.3`, поэтому если нужна строго последняя версия, проверь обновление пакета через Unity Asset Store/Package Manager. Ниже инструкция написана по фактически импортированным файлам проекта.

## 1. Что уже есть в папке ассета

Корень ассета:

- `Animations` - animator controllers, исходный FBX с анимациями и вынесенные `.anim` клипы.
- `Configs` - `SlotLibrary.asset`, главный конфиг доступных частей персонажа.
- `Materials` - материалы `Color.mat`, `Glass.mat`, `Emission.mat`, `Floor_checker.mat`.
- `Meshes` - исходные FBX для тела, лиц, одежды, обуви, волос, шапок, аксессуаров.
- `Prefabs` - готовые prefab-варианты тех же частей.
- `Render_Pipeline_Convert` - документация по конвертации материалов между URP/Built-in/HDRP.
- `Scenes` - демонстрационная сцена `Demonstration.unity`.
- `Scripts` - runtime-скрипты, editor-окно кастомизации, randomizer, FacePicker.
- `Textures` - `Textures.png`, основная текстура пакета.

### 1.1. Важные ассеты

- Базовый персонаж: `Prefabs/Base_Mesh.prefab`.
- Базовый FBX: `Meshes/Base_Mesh.fbx`.
- Библиотека слотов: `Configs/SlotLibrary.asset`.
- Основной runtime animator controller: `Animations/Animation_Controllers/Character_Movement.controller`.
- Дополнительный controller: `Animations/AnimationController.controller`.
- FBX с набором анимаций: `Animations/Animation_Mesh/Aminset_Basic.fbx`.
- Демонстрационная сцена: `Scenes/Demonstration.unity`.

### 1.2. Доступные prefab-части в текущем импорте

База:

- `Base_Mesh.prefab`
- `Body/Body_010.prefab`

Full-body/костюмы:

- `Costumes/Costume_13_001.prefab`
- `Costumes/Costume_13_002.prefab`

Лица:

- `Faces/Male_emotion_usual_001.prefab`
- `Faces/Male_emotion_happy_002.prefab`
- `Faces/Male_emotion_angry_003.prefab`

Аксессуары лица:

- `Face Accessories/Clown_nose_001.prefab`
- `Face Accessories/Headphones_002.prefab`
- `Face Accessories/Mustache_003.prefab`
- `Face Accessories/Mustache_011.prefab`
- `Face Accessories/Pacifier_001.prefab`

Очки:

- `Glasses/Glasses_004.prefab`
- `Glasses/Glasses_006.prefab`

Перчатки:

- `Gloves/Gloves_006.prefab`
- `Gloves/Gloves_014.prefab`

Волосы:

- `Hairstyle/Hairstyle_Male_001.prefab`
- `Hairstyle/Hairstyle_Male_005.prefab`
- `Hairstyle Single/Hairstyle_Male_Single_006.prefab`

Шапки:

- `Hat/Hat_010.prefab`
- `Hat Single/Hat_Single_008.prefab`
- `Hat Single/Hat_Single_013.prefab`
- `Hat Single/Hat_Single_016.prefab`

Одежда и обувь:

- `Mascots/Mascot_002.prefab`
- `Outfit/Outfit_010.prefab`
- `Outwear/Outwear_004.prefab`
- `Outwear/Outwear_043.prefab`
- `Outwear/Outwear_050.prefab`
- `Pants/Pants_009.prefab`
- `Pants/Pants_010.prefab`
- `Shorts/Shorts_003.prefab`
- `Socks/Socks_008.prefab`
- `Shoes/Shoe_Slippers_002.prefab`
- `Shoes/Shoe_Slippers_005.prefab`
- `Shoes/Shoe_Sneakers_009.prefab`

### 1.3. Анимации

В `Other_Animations` лежат отдельные `.anim` клипы:

- `Idle_Breathing`
- `Idle_Look_Around`
- `Idle_Relaxed`
- `Jump_End`
- `Jump_Loop`
- `Jump_Start`
- `Run_Backward`
- `Run_Forward`
- `Run_Left`
- `Run_Right`
- `Strafe_Left`
- `Strafe_Right`
- `Walk_Backward`
- `Walk_Forward`

В `Aminset_Basic.fbx` также импортированы дополнительные clips, включая атаки, crouch, dodge, death, hit reaction, fall, climb ladder. FBX импортирован как Humanoid (`animationType: 3`), поэтому персонажа можно ретаргетить на другие Humanoid-анимации при корректном Avatar.

## 2. Что делают скрипты ассета

### 2.1. Runtime-скрипты

`Scripts/Character_Controller/CharacterMover.cs`

- Требует `CharacterController` и `Animator`.
- Двигает персонажа через `CharacterController.Move`.
- Управляет гравитацией и прыжком.
- Поворачивает персонажа к target-точке камеры.
- Записывает параметры animator:
  - `Hor` - горизонтальное движение.
  - `Vert` - вертикальное движение.
  - `State` - 0 для walk, 1 для run.
  - `IsJump` - true в воздухе/при прыжке.
- В `OnAnimatorIK` направляет взгляд персонажа на target камеры.

`Scripts/Character_Controller/MovePlayerInput.cs`

- Читает старый `UnityEngine.Input`.
- Использует оси/кнопки:
  - `Horizontal`
  - `Vertical`
  - `Jump`
  - `Mouse X`
  - `Mouse Y`
  - `Mouse ScrollWheel`
- Run включается клавишей `LeftShift`.
- Ищет камеру так:
  - если поле камеры задано в инспекторе, использует его;
  - иначе берет `Camera.main` и пытается получить на ней `PlayerCamera`.
- Передает ввод в `CharacterMover` и `PlayerCamera`.

`Scripts/Character_Controller/PlayerCamera.cs`

- Абстрактная база камеры.
- Хранит player transform, чувствительность мыши, zoom, ограничения угла.
- Создает служебный объект `Target_<CameraName>`.
- `Target` используется как точка, на которую смотрит/разворачивается персонаж.

`Scripts/Character_Controller/ThirdPersonCamera.cs`

- Реальная third-person камера.
- В `LateUpdate` плавно двигает камеру вокруг игрока.
- Смотрит в точку над игроком.
- Обновляет target-точку перед камерой.
- В demo-сцене висит на `Main Camera` с настройками:
  - `Sensitivity X = 0.05`
  - `Sensitivity Y = 0.03`
  - `Zoom = 0.1`
  - `Sensitivity Zoom = 0.2`
  - `Min Angle = 0`
  - `Max Angle = 60`
  - `Offset = 1.5`
  - `Camera Speed = 70`

`Scripts/FaceManagement/FacePicker.cs`

- Компонент для переключения меша лица.
- При сохранении prefab добавляется на корневой объект персонажа.
- Внутри ищет дочерний объект, имя которого начинается с `Face`, и берет его `SkinnedMeshRenderer`.
- Метод `PickFace(FaceType faceType)` меняет `sharedMesh` лица.
- Метод `HasFace(FaceType face)` проверяет, есть ли такая эмоция в текущем наборе.

`Scripts/FaceManagement/FaceType.cs`

Поддерживаемые enum-значения:

- `Anger`
- `Angry`
- `Compassion`
- `Confuse`
- `Evil`
- `Happy`
- `Impatience`
- `Madness`
- `Neutral`
- `Sad`
- `Surprised`
- `Usual`

В текущем бесплатном наборе фактически видны male-лица `Usual`, `Happy`, `Angry`.

`Scripts/FaceManagement/FaceMesh.cs`

- Обертка над `Mesh`.
- Тип лица вычисляется из имени меша: скрипт берет третью часть имени после `_`.
- Для имени `Male_emotion_happy_002` тип станет `Happy`.

`Scripts/Extensions/StringExtensions.cs`

- Вспомогательный метод `ToCapital()`.
- Используется при парсинге имен лиц/слотов.

### 2.2. Editor-скрипты

`Scripts/Editor/CharacterCustomizationWindow.cs`

- Главное окно сборки персонажа.
- Пункт меню: `Tools > ithappy > Creative_Characters_FREE > Character Customization`.
- При открытии создает Unity Layer `Character Preview` через `ProjectSettings/TagManager.asset`.
- Показывает preview персонажа.
- Позволяет выбирать части, рандомизировать и сохранять итоговый prefab.
- При сохранении:
  - инстанцирует `Base_Mesh`;
  - подставляет выбранные `SkinnedMeshRenderer.sharedMesh`;
  - назначает основной материал `Color.mat`;
  - добавляет `FacePicker`;
  - назначает `Character_Movement.controller`;
  - добавляет `CharacterController`;
  - добавляет `CharacterMover`;
  - добавляет `MovePlayerInput`;
  - сохраняет prefab в `Assets/ithappy/Creative_Characters_FREE/Saved_Characters/<введенная_папка>/Character.prefab`.

`Scripts/Editor/PartsEditor.cs`

- Рисует правую часть окна кастомизации.
- Для каждого слота показывает:
  - имя;
  - preview;
  - toggle `Enabled`;
  - кнопки `<` и `>`;
  - счетчик `текущий/всего`.

`Scripts/Editor/SlotLibrary.cs`

- `ScriptableObject`, который описывает доступные слоты.
- Главное место, куда добавляются новые части персонажа.
- Содержит:
  - `FullBodyCostumes`;
  - `Slots`;
  - `SlotEntry`;
  - `SlotGroupEntry`.

`Scripts/Editor/Character/*`

- `CustomizableCharacter` - модель собираемого персонажа.
- `Slot` - обычный слот одной части.
- `FullBodySlot` - слот полного костюма.
- `SlotBase` - общая база.
- `SlotGroup` - группа вариантов.
- `SlotVariant` - конкретный mesh-вариант.
- `FullBodyVariant` - набор мешей для full-body костюма.
- `PreviewCreator` - создает preview-объект для окна.
- `SlotLibraryLoader` - грузит `Configs/SlotLibrary.asset`.

`Scripts/Editor/SlotValidation/*`

- `FullBodyToggledRule` - если включить `FullBody`, все обычные части кроме `Body` и `Faces` выключаются.
- `SlotToggledRule` - если включить любую обычную часть, `FullBody` выключается.
- `SlotValidator` - применяет эти правила.

`Scripts/Editor/Randomizer/*`

- `RandomCharacterGenerator` вызывает шаги в порядке:
  - Face
  - Body
  - Costumes
  - Mascots
  - Outfit
  - Outwear
  - Pants
  - Shorts
  - Socks
  - Hat
  - HatSingle
  - FaceAccessories
  - Glasses
  - Shoes
  - HairstyleSingle
  - Hairstyle
  - Gloves
- `BodyStep` и `FaceStep` всегда активны.
- `CostumesStep` срабатывает с вероятностью `0.2`.
- `SocksStep` срабатывает с вероятностью `0.5`.
- Остальные опциональные шаги наследуют вероятность `0.35`.
- После выбора full-body костюма остальные группы становятся недоступны.

`Scripts/Editor/FaceEditor/*`

- `FaceLoader` при сохранении prefab собирает все face meshes из `Meshes/Faces`.
- Лица группируются по полу и варианту имени.
- `FacePickerEditor` добавляет popup `Face` в инспекторе `FacePicker`.

`Scripts/Editor/MaterialManagement/*`

- `MaterialProvider` предоставляет материалы.
- При сохранении персонажа сейчас назначается `MainColor`, то есть `Color.mat` или fallback `Material.mat`, если такой появится.
- `Glass.mat` и `Emission.mat` существуют, но автоматически не назначаются в `SavePrefab`; если нужны стеклянные очки или emissive-детали, назначай материал вручную на нужные renderer после сохранения.

`Scripts/Editor/AssetsPath.cs`

- Важный hardcoded root:
  - `Assets/ithappy/Creative_Characters_FREE/`
- Если перенести папку ассета в другое место, editor-окно может перестать находить `SlotLibrary`, материалы, анимационный controller и базовый mesh.
- Метод поиска root есть в коде, но вызов `FindRoot()` закомментирован.

## 3. Подготовка проекта

1. Открой проект в Unity 6000.5.2f1.
2. Дождись окончания import/compile.
3. Проверь Console. До открытия окна кастомизации не должно быть compile errors.
4. Проверь, что папка есть именно здесь:

   `Assets/ithappy/Creative_Characters_FREE`

5. Проверь render pipeline:
   - В проекте установлен URP (`com.unity.render-pipelines.universal` версии `17.5.0`).
   - Ассет изначально рассчитан на URP-материалы, поэтому для текущего проекта конвертация обычно не нужна.
   - Если проект Built-in или HDRP, смотри `Render_Pipeline_Convert/Documentation.txt`.
   - В текущей локальной папке виден только `Documentation.txt`; если `.unitypackage` для конвертации отсутствует, обнови пакет через Asset Store или конвертируй материалы вручную.

6. Проверь input:
   - `MovePlayerInput` использует старый `UnityEngine.Input`.
   - В текущем проекте установлен новый Input System.
   - В `ProjectSettings/ProjectSettings.asset` сейчас указан `activeInputHandler: 1`, то есть проект может быть настроен на новый Input System.
   - Для готового скрипта движения выставь `Edit > Project Settings > Player > Other Settings > Active Input Handling` в `Both` или `Input Manager (Old)`.
   - После изменения Unity обычно попросит перезапуск редактора. Согласись.

7. Проверь старые оси Input Manager:
   - `Edit > Project Settings > Input Manager`.
   - Должны существовать `Horizontal`, `Vertical`, `Jump`, `Mouse X`, `Mouse Y`, `Mouse ScrollWheel`.
   - В стандартном Unity-проекте они обычно есть.

## 4. Быстрый путь: собрать персонажа через встроенное окно

1. В Unity открой:

   `Tools > ithappy > Creative_Characters_FREE > Character Customization`

2. Unity откроет окно `Character Customization`.
3. При первом открытии окно добавит layer:

   `Character Preview`

   Это изменение попадет в `ProjectSettings/TagManager.asset`. Это нормально: layer нужен для preview-камеры.

4. В левой части окна находится preview персонажа.
5. Ниже preview есть поле:

   `Prefab folder:`

   Под ним показана базовая папка:

   `Assets/ithappy/Creative_Characters_FREE/Saved_Characters/`

6. Поле ввода под этим путем - это подпапка для сохранения. Примеры:
   - оставить пустым - prefab сохранится прямо в `Saved_Characters`;
   - ввести `Player` - prefab сохранится в `Saved_Characters/Player`;
   - ввести `NPC/Village` - prefab сохранится в `Saved_Characters/NPC/Village`.

7. Справа отображаются слоты персонажа.
8. Для каждого слота:
   - `Enabled` включает/выключает часть;
   - `<` выбирает предыдущий вариант;
   - `>` выбирает следующий вариант;
   - счетчик показывает `текущий вариант / всего вариантов`.

9. Слоты `Body` и `Faces` всегда включены. Их нельзя выключить через UI, потому что это базовые части персонажа.
10. Слот `Full Body` конфликтует с обычными частями:
    - если включить `Full Body`, отключатся обычные одежда/аксессуары;
    - если включить любую обычную часть, `Full Body` выключится.

11. Собери персонажа:
    - выбери `Body`;
    - выбери `Faces`;
    - реши, используешь ли `Full Body` или набор отдельных частей;
    - если не используешь `Full Body`, включи нужные шапки, волосы, очки, одежду, обувь, перчатки, аксессуары;
    - проверь preview.

12. Для случайной сборки нажми `Randomize`.
13. Если randomize дал неудачный результат, нажми `Randomize` еще раз.
14. Кнопка `Last` возвращает предыдущую комбинацию. Окно хранит до 4 последних комбинаций.

15. Когда внешний вид готов, нажми `Save Prefab`.
16. Unity создаст папку `Saved_Characters`, если ее еще нет.
17. Итоговый prefab появится по пути:

   `Assets/ithappy/Creative_Characters_FREE/Saved_Characters/<твоя_подпапка>/Character.prefab`

18. Если `Character.prefab` уже есть, Unity создаст уникальное имя через `AssetDatabase.GenerateUniqueAssetPath`, например `Character 1.prefab`.

## 5. Что именно появляется в сохраненном prefab

Сохраненный prefab - это уже не просто модель. Окно автоматически добавляет игровые компоненты:

- `Animator`
- `CharacterController`
- `CharacterMover`
- `MovePlayerInput`
- `FacePicker`

### 5.1. Animator

Окно назначает:

`Animations/Animation_Controllers/Character_Movement.controller`

У controller есть параметры:

- `Vert` - Float.
- `Hor` - Float.
- `State` - Float.
- `IsJump` - Bool.

`CharacterMover` по умолчанию пишет именно в эти параметры. Если заменить Animator Controller, нужно либо сохранить такие же параметры, либо изменить поля в компоненте `CharacterMover`.

`applyRootMotion` при сохранении выключается (`false`). Движение делает `CharacterController`, а не root motion.

### 5.2. CharacterController

Окно добавляет `CharacterController` с настройками:

- `center = (0, 0.95, 0)`
- `radius = 0.4`
- `height = 1.8`

После помещения персонажа в свою сцену проверь:

- нижняя точка capsule стоит на земле;
- персонаж не висит в воздухе;
- персонаж не проваливается;
- `Step Offset` и `Slope Limit` подходят под геометрию уровня.

### 5.3. CharacterMover

Основные поля:

- `Walk Speed` - скорость ходьбы.
- `Run Speed` - скорость бега.
- `Rotate Speed` - скорость разворота.
- `Space`:
  - `Self` - движение относительно направления камеры/target;
  - `World` - движение относительно мировых осей.
- `Jump Height` - сила/высота прыжка.
- `Horizontal ID` - имя animator-параметра, по умолчанию `Hor`.
- `Vertical ID` - имя animator-параметра, по умолчанию `Vert`.
- `State ID` - имя animator-параметра, по умолчанию `State`.
- `Jump ID` - имя animator-параметра, по умолчанию `IsJump`.
- `Look Weight` - настройки IK-взгляда.

Практические стартовые значения:

- `Walk Speed = 1`
- `Run Speed = 4`
- `Rotate Speed = 90`
- `Space = Self`
- `Jump Height = 5`

Примечание: в коде `OnValidate()` пересчитывает скорость с делением на `3.6`, но `Awake()` передает исходные значения напрямую. Поэтому лучше задать скорости до Play Mode, протестировать в сцене и не считать поле строго километрами в час.

### 5.4. MovePlayerInput

Основные поля:

- `Horizontal Axis = Horizontal`
- `Vertical Axis = Vertical`
- `Jump Button = Jump`
- `Run Key = LeftShift`
- `Camera` - ссылка на `PlayerCamera`; можно оставить пустой, если в сцене есть `Main Camera` с компонентом `ThirdPersonCamera`.
- `Mouse X = Mouse X`
- `Mouse Y = Mouse Y`
- `Mouse Scroll = Mouse ScrollWheel`

Если персонаж не двигается:

- проверь `Active Input Handling`;
- проверь наличие осей в Input Manager;
- проверь, что Game view активен;
- проверь, что на объекте есть `CharacterMover`;
- проверь, что `MovePlayerInput` включен.

### 5.5. FacePicker

Компонент появляется, если у персонажа есть renderer лица с mesh.

В Inspector у `FacePicker` появится поле `Face`, где можно выбрать доступную эмоцию. Для текущего набора обычно доступны:

- `Usual`
- `Happy`
- `Angry`

Переключение из кода:

```csharp
using ithappy.Creative_Characters_FREE.CharacterCustomizationTool.FaceManagement;

public class ExampleFaceSwitch : MonoBehaviour
{
    [SerializeField] private FacePicker facePicker;

    public void SetHappy()
    {
        if (facePicker.HasFace(FaceType.Happy))
        {
            facePicker.PickFace(FaceType.Happy);
        }
    }
}
```

Важно: `FacePicker` парсит тип лица из имени mesh. Для новых лиц используй тот же стиль имени:

`Male_emotion_happy_002`

Третья часть (`happy`) должна соответствовать `FaceType.Happy`.

## 6. Настройка персонажа в игровой сцене

### 6.1. Добавить персонажа в сцену

1. Найди сохраненный prefab:

   `Assets/ithappy/Creative_Characters_FREE/Saved_Characters/.../Character.prefab`

2. Перетащи его в Hierarchy.
3. Поставь позицию, например:

   `Position = (0, 0, 0)`

4. Убедись, что под персонажем есть объект с Collider:
   - Plane с MeshCollider;
   - TerrainCollider;
   - уровень с colliders.

Без collider `CharacterController` будет падать под действием gravity.

### 6.2. Настроить камеру

1. В сцене должна быть камера с tag `MainCamera`.
2. На камере должен быть компонент `ThirdPersonCamera`.
3. Если камеры нет:
   - создай `GameObject > Camera`;
   - назови `Main Camera`;
   - поставь tag `MainCamera`;
   - добавь component `ThirdPersonCamera`.

4. Рекомендуемые стартовые настройки как в demo-сцене:

   - `Sensitivity X = 0.05`
   - `Sensitivity Y = 0.03`
   - `Zoom = 0.1`
   - `Sensitivity Zoom = 0.2`
   - `Min Angle = 0`
   - `Max Angle = 60`
   - `Offset = 1.5`
   - `Camera Speed = 70`

5. Если `MovePlayerInput.Camera` пустой, скрипт сам найдет:

   `Camera.main.GetComponent<PlayerCamera>()`

6. В Play Mode камера привяжется к transform персонажа через `m_Camera.SetPlayer(transform)`.

### 6.3. Проверить управление

В Play Mode:

- `W/S` или вертикальная ось - движение вперед/назад.
- `A/D` или горизонтальная ось - движение влево/вправо.
- `LeftShift` - бег.
- `Space` или button `Jump` - прыжок.
- мышь - вращение камеры.
- колесо мыши - zoom.

Если используешь геймпад или новый Input System, готовый `MovePlayerInput` не будет читать `InputAction` напрямую. Тогда есть два варианта:

1. Оставить `Active Input Handling = Both` и использовать старые оси.
2. Написать новый компонент ввода, который вызывает:

```csharp
characterMover.SetInput(axis, cameraTarget, isRun, isJump);
playerCamera.SetInput(mouseDelta, scroll);
```

## 7. Настройка материалов

По умолчанию при сохранении prefab всем включенным SkinnedMeshRenderer назначается `Color.mat`.

Что проверить после сохранения:

1. Открой prefab.
2. Пройди по дочерним renderer.
3. Проверь материалы:
   - тело/одежда обычно `Color.mat`;
   - очкам при необходимости можно вручную назначить `Glass.mat`;
   - emissive-деталям при необходимости можно вручную назначить `Emission.mat`.

Если материалы розовые:

- проект использует не тот render pipeline;
- shader материала не поддерживается текущим pipeline;
- URP/HDRP asset не назначен в Graphics/Quality settings;
- пакет был импортирован не полностью.

Для текущего проекта URP конвертация обычно не нужна.

## 8. Использование demonstration-сцены

Сцена:

`Assets/ithappy/Creative_Characters_FREE/Scenes/Demonstration.unity`

В ней есть:

- `Directional Light`
- `Floor`
- `Main Camera` с tag `MainCamera`
- `ThirdPersonCamera` на Main Camera

Сцена полезна как reference для камеры и пола. Если хочешь проверить нового персонажа:

1. Открой `Demonstration.unity`.
2. Перетащи сохраненный `Character.prefab` в сцену.
3. Поставь персонажа над `Floor`.
4. Нажми Play.
5. Если камера не следует за персонажем, проверь:
   - tag камеры `MainCamera`;
   - компонент `ThirdPersonCamera`;
   - компонент `MovePlayerInput` на персонаже;
   - поле `Camera` в `MovePlayerInput`, если автоматический поиск не сработал.

## 9. Подробный рабочий сценарий для первого персонажа

1. Открой Unity.
2. Убедись, что Console без ошибок.
3. Установи `Active Input Handling = Both`.
4. Перезапусти Unity, если редактор попросит.
5. Открой `Tools > ithappy > Creative_Characters_FREE > Character Customization`.
6. В поле подпапки введи:

   `Player`

7. Настрой внешний вид:
   - оставь `Body` включенным;
   - выбери `Faces`;
   - выключи `Full Body`, если хочешь собирать одежду по частям;
   - включи нужные слоты одежды и аксессуаров;
   - используй `<` и `>` для выбора вариантов.

8. Нажми `Save Prefab`.
9. Найди prefab:

   `Assets/ithappy/Creative_Characters_FREE/Saved_Characters/Player/Character.prefab`

10. Открой нужную игровую сцену.
11. Добавь на сцену пол/уровень с collider.
12. Перетащи `Character.prefab` на сцену.
13. Создай или настрой `Main Camera`.
14. Добавь на камеру `ThirdPersonCamera`.
15. Установи tag камеры `MainCamera`.
16. Нажми Play.
17. Проверь:
    - персонаж стоит на земле;
    - камера следует;
    - WASD двигает;
    - Shift включает run;
    - Space прыгает;
    - анимации blend-ятся;
    - колесо мыши меняет zoom.

18. Открой prefab и проверь `FacePicker`.
19. Выбери `Happy`, `Angry` или `Usual` в Inspector.
20. Если все работает, переименуй prefab в проектное имя, например:

   `PlayerCharacter.prefab`

## 10. Как добавить нового персонажа/NPC

Для второго персонажа лучше не редактировать уже сохраненный prefab, а создать новый через окно:

1. Открой `Character Customization`.
2. Введи другую подпапку, например:

   `NPC/Villager_01`

3. Собери внешний вид.
4. Нажми `Save Prefab`.
5. Переименуй итоговый prefab, если нужно.
6. Для NPC можно отключить `MovePlayerInput`, если им будет управлять AI.
7. Для AI оставь:
   - `Animator`;
   - `CharacterController`, если движение будет через controller;
   - `CharacterMover`, если AI будет вызывать `SetInput`;
   - `FacePicker`, если нужны эмоции.

## 11. Как управлять персонажем своим кодом

Если не нужен `MovePlayerInput`, его можно отключить или удалить и передавать input самому.

Пример:

```csharp
using UnityEngine;
using ithappy.Creative_Characters_FREE.Controller;

public class AiCharacterDriver : MonoBehaviour
{
    [SerializeField] private CharacterMover mover;
    [SerializeField] private Transform lookTarget;

    private void Reset()
    {
        mover = GetComponent<CharacterMover>();
    }

    private void Update()
    {
        var axis = new Vector2(0f, 1f);
        var target = lookTarget != null
            ? lookTarget.position
            : transform.position + transform.forward * 10f;

        var isRun = false;
        var isJump = false;

        mover.SetInput(axis, target, isRun, isJump);
    }
}
```

Для player-контроллера на новом Input System сделай аналогично: считай `Vector2 move`, `Vector2 look`, `bool run`, `bool jump`, затем вызови `CharacterMover.SetInput` и `PlayerCamera.SetInput`.

## 12. Как добавить новые части в SlotLibrary

Это нужно, если ты расширяешь бесплатный набор своими FBX/prefab.

1. Подготовь FBX с тем же humanoid skeleton/skin setup.
2. Импортируй модель в подходящую папку внутри `Meshes`.
3. Создай prefab в подходящей папке внутри `Prefabs`.
4. Убедись, что внутри prefab есть `SkinnedMeshRenderer`.
5. Открой:

   `Assets/ithappy/Creative_Characters_FREE/Configs/SlotLibrary.asset`

6. Добавь prefab в нужный `SlotEntry`.
7. Выбери правильный `SlotType`:
   - `Body`
   - `Faces`
   - `FullBody`
   - `Glasses`
   - `Gloves`
   - `Hairstyle`
   - `Hat`
   - `Mustache`
   - `Outerwear`
   - `Pants`
   - `Shoes`
   - `TShirt`
   - `Accessories`

8. Выбери правильный `GroupType`:
   - `Body`
   - `Costumes`
   - `FaceAccessories`
   - `Faces`
   - `Glasses`
   - `Gloves`
   - `Hairstyle`
   - `HairstyleSingle`
   - `Hat`
   - `HatSingle`
   - `Mascots`
   - `Outfit`
   - `Outwear`
   - `Pants`
   - `Shoes`
   - `Shorts`
   - `Socks`

9. Сохрани asset.
10. Закрой и заново открой окно `Character Customization`.
11. Проверь, что новая часть появилась в нужном слоте.

Важно: обычный `Slot` берет mesh так:

```csharp
prefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh
```

Если в prefab нет `SkinnedMeshRenderer`, окно не сможет использовать эту часть.

## 13. Как добавить новые лица

1. Новый face mesh должен лежать в:

   `Assets/ithappy/Creative_Characters_FREE/Meshes/Faces`

2. Имя должно соответствовать текущему парсингу:

   `<Gender>_emotion_<emotion>_<variant>`

3. Пример:

   `Male_emotion_happy_004`

4. `<emotion>` после `ToCapital()` должен совпасть с `FaceType`.
5. Если нужна новая эмоция, которой нет в enum, добавь ее в:

   `Scripts/FaceManagement/FaceType.cs`

6. Создай prefab лица в:

   `Prefabs/Faces`

7. Добавь prefab в `SlotLibrary.asset` в слот `Faces`.
8. Собери и сохрани персонажа заново.

Если `FacePicker.PickFace` выдает `Face not found`, значит в наборе текущего персонажа нет mesh для этой эмоции.

## 14. Частые проблемы и решения

### 14.1. Нет пункта меню Character Customization

Проверь:

- Console содержит compile errors.
- Папка ассета не переехала из `Assets/ithappy/Creative_Characters_FREE`.
- `Scripts/Editor/CharacterCustomizationWindow.cs` импортирован.
- Assembly Definition `ithappy.Creative_Characters_FREE.asmdef` не отключен.

### 14.2. Окно открылось, но preview пустой

Проверь:

- существует `Meshes/Base_Mesh.fbx`;
- существует `Prefabs/Base_Mesh.prefab`;
- существует `Configs/SlotLibrary.asset`;
- материалы не розовые;
- layer `Character Preview` создан;
- Console не содержит ошибок `Sequence contains no elements`.

### 14.3. Save Prefab падает с ошибкой поиска child

В `SavePrefab()` код ищет child по началу имени slot type:

```csharp
character.transform.Cast<Transform>().First(t => t.name.StartsWith(mesh.Item1.ToString()))
```

Если структура базового mesh изменилась, имена дочерних объектов должны начинаться с `Body`, `Faces`, `Hat`, `Pants`, `Shoes` и т.д. Иначе сохранение не найдет нужный renderer.

### 14.4. Персонаж не двигается

Проверь:

- `Active Input Handling = Both` или старый Input Manager.
- В сцене есть активная `Main Camera`.
- На камере есть `ThirdPersonCamera`.
- На персонаже есть `MovePlayerInput`.
- На персонаже есть `CharacterMover`.
- На персонаже есть `CharacterController`.
- Под персонажем есть collider.

### 14.5. Unity пишет ошибку про UnityEngine.Input

Это означает, что проект работает только на новом Input System, а скрипт читает старый `UnityEngine.Input`.

Решение:

1. `Edit > Project Settings > Player > Other Settings`.
2. Найди `Active Input Handling`.
3. Выбери `Both`.
4. Перезапусти Unity.

Альтернатива: переписать `MovePlayerInput` на `InputAction`.

### 14.6. Камера не следует за персонажем

Проверь:

- tag камеры строго `MainCamera`;
- на камере есть `ThirdPersonCamera`;
- в сцене только одна активная main camera;
- поле `Camera` в `MovePlayerInput` можно назначить вручную;
- персонаж активен в сцене.

### 14.7. Анимации не играют

Проверь:

- у Animator назначен `Character_Movement.controller`;
- в controller есть параметры `Vert`, `Hor`, `State`, `IsJump`;
- `CharacterMover` использует те же имена параметров;
- Avatar у модели Humanoid и валиден;
- `applyRootMotion` выключен.

### 14.8. Персонаж проваливается

Проверь:

- у пола есть Collider;
- персонаж стартует над полом;
- `CharacterController.center`, `height`, `radius` подходят модели;
- слой пола не исключен из physics collision matrix.

### 14.9. Материалы розовые

Проверь:

- проект URP/HDRP/Built-in соответствует shader материалов;
- URP pipeline asset назначен в Graphics и Quality;
- материалы `Color.mat`, `Glass.mat`, `Emission.mat` существуют;
- пакет импортирован полностью.

### 14.10. После переноса папки ассета все ломается

`AssetsPath.cs` содержит hardcoded root:

`Assets/ithappy/Creative_Characters_FREE/`

Лучшее решение - не переносить папку. Если перенос нужен, обнови `AssetsPath._root` или включи/доработай поиск root в `CharacterCustomizationWindow.FindRoot()`.

## 15. Финальный чеклист готового персонажа

Перед тем как использовать персонажа в игре, проверь:

- prefab лежит в `Saved_Characters`;
- prefab переименован понятным именем;
- `Animator` назначен;
- `Character_Movement.controller` назначен;
- параметры animator совпадают с `CharacterMover`;
- `CharacterController` capsule совпадает с моделью;
- `CharacterMover` включен;
- `MovePlayerInput` включен только для player-персонажа;
- для NPC input-компонент отключен или заменен AI-драйвером;
- `FacePicker` работает;
- материалы не розовые;
- камера в сцене имеет `ThirdPersonCamera`;
- камера имеет tag `MainCamera`;
- input работает в Play Mode;
- персонаж не проваливается;
- анимации walk/run/jump проигрываются.

## 16. Минимальный рецепт в одну страницу

1. Открой Unity 6000.5.2f1.
2. Установи `Active Input Handling = Both`.
3. Открой `Tools > ithappy > Creative_Characters_FREE > Character Customization`.
4. Собери внешний вид персонажа.
5. Введи подпапку, например `Player`.
6. Нажми `Save Prefab`.
7. Перетащи `Saved_Characters/Player/Character.prefab` в сцену.
8. Убедись, что в сцене есть пол с collider.
9. На `Main Camera` добавь `ThirdPersonCamera`.
10. Проверь tag `MainCamera`.
11. Нажми Play.
12. Проверь WASD, Shift, Space, мышь и колесо.

