# README #

### Процесс сборки: 

* Настроить все поля в разделе верхнего меню редактора юнити Solitaire -> Bundle Settings
* переключиться на нужную платформу (ios \ android) в разделе File -> Build Settings…
* выбрать нужный пресет в меню Solitaire -> Bundle Settings, для верности нажать Apply что бы применить текущий пресет к проекту
* собирать

### Локализация: 

Если в файле языков оформить остальные языки точно так же как русский и английский (использовать те же идентификаторы фраз) то можно просто заменить файл по адресу Assets/StreamingAssets/languages.xml и все языки будут работать.

### Иконки:

Для добавления иконок нужно  создать подкаталог в папке Assets/textures/icons. Все иконки должны быть в формате png, а имена файлов должны соответствовать размерам иконки (например 190.png). 

Можно указать только одну или несколько иконок, тогда редактор unity сам проскоблит картинку под все необходимые размеры. 

В идеальном случае нужно создать иконки под каждый размер, положить в папку и указать эту папку в Bundle Settings.

Полный список размеров под все платформы:
192, 180,167,152,144,120,114,96,87,80,76,72,58,57,48,40,36,29

После нажатия Apply в Bundle Settings все иконки, что найдет скрипт будут применены к проекту.

### Splash screen:

Добавить картинку в проект (например в папку Assets/textures/splash/variants) и указать ее в Bundle Settings. 

После нажатия Apply в Bundle Settings сплэш будет применен к проекту. (Оригинальный splash.png из Assets/textures не трогать !!!)

### Keystore для Android:

 Путь к файлу с ключом нужно указывать относительно папки проекта, а не папки Assets !!!