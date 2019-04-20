# ua-elections-2019

# Протоколи

## Зйомка протоколів

Моя особиста рекомендація - використання додатку **Microsoft Office Lens**.  
Він має можливість автоматично вирізати необхідну область з документом та корректувати освітлення документу. Потестуйте, вам сподобається, а волонтери швидше опрацюють підготовлені протоколи.
Приклади роботи додатка:

- порівння фото з телефону та з додатку - https://bdfss.blob.core.windows.net/shots/2019-04-19_17-57-25-UTC_2.0-36e2f0ce-626b-46fb-911f-3557ac3a3891.jpg
- фото з додатку окремо - https://bdfss.blob.core.windows.net/shots/2019_04_19_17_56_Office_Lens.jpg

Посилання на **Microsoft Office Lens**
- Android https://play.google.com/store/apps/details?id=com.microsoft.office.officelens&hl=uk
- ios https://itunes.apple.com/us/app/microsoft-office-lens-pdf-scan/id975925059?mt=8

**Відео роботи додатка**, будь ласка, перейдіть за посиланням - https://youtu.be/fuBQ7rVKCWo

## Підготовка протоколів

Ще одна особиста рекомендація, яке не займе багато часу - перез завантаженням протоколів на сайт перевірте орієнтацію фото, якщо у вас windows 10, у стандартному переглядачі фотографій комбінація клавіш _CTRL+R_ оберне фото на 90 градусів. Знову ж таки, це пришвидшіть швидкість опрацювання волонтерами протоколів, а у вас займе до 1 секунди на кожне фото.

Відео процессу, будь ласка, перейдіть за посиланням - https://bdfss.blob.core.windows.net/shots/2019-04-20_20-30-39-UTC_2.0-9d449fad-8456-4878-a323-aef18c12b5e1.mp4

- - -

# Завантаження протоколів на сайт https://e-vybory.org за допомогою додатку ResUp

 __Будь ласка, дотримуйтесь правил користування додатком, додаток не розрахований на можливі помилки користувачів та відноситься до них нетолерантно, єдина функція цього додатку - прибрати необхідність вводити велику кількість данних на сайт у руками під час завантаження протоколів у великому обсязі__


# Інсталляція додатку для завантаження протоколів на сайт

Нові версії додатку ви зможете завантажити за цим посиланням https://github.com/bondarenkod/ua-elections-2019/releases

Для роботи додатку необхідно інсталлювати .NET Framework 4.7.2, інсталлятор вже включено до пакету - файл NDP472-KB4054531-Web.exe.
Якщо у вас встановлені останні апдейти для ОС - скоріше за все .NET Framework 4.7.2 у вас вже проінсталльовано. Додаток протестовано на Windows 7-32 та 10 з останніми апдейтами.

# Використання додатку

0. Після успішного запуску додатку (файл **ResUp.exe**) ви повинні бачити екран на скріншоті нижче. Це означає, що все працює нормально. За замовченням відкривається стартова сторінка https://e-vybory.org/.
![alt](https://bdfss.blob.core.windows.net/shots/2019-04-20_19-10-23-ffec0c94-689a-4954-843f-c300b82820f9-ResUp.png)
   

1. Таби **browser** та **logic** 
![alt](https://bdfss.blob.core.windows.net/shots/2019-04-20_19-20-58-66af35ab-1949-4ad4-983c-151875ca93eb-ResUp.png)   

- **browser** - містить у собі вбудований браузер на базі проекту https://github.com/cefsharp/CefSharp, а також дві кнопки - home та my-docs 
![alt](https://bdfss.blob.core.windows.net/shots/2019-04-20_19-21-07-f15c0376-baad-49f3-9983-3c795d97e754-ResUp.png)     
    - **home** - відкриває домашню сторінку сайту https://e-vybory.org/
    - **my-docs** - відкриває домашню сторінку сайту https://e-vybory.org/my-docs
  Сторінка **my-docs** відкривається тільки для залогінених користувачів. Для цього необіхідно авторизуватись.

- **logic** - містить все необхідне для завантаження протоколів на сайт.

3. Логін у свій профіль на сайті https://e-vybory.org/ через додаток.

Якщо ви ще не логінились на сайт або ваша сессія вже скінчилась, вам необхідно зайти на сайт у свій профіль. Для цього я зробив коротеньке **відео**, яке ви можете подивитись за посиланням: https://youtu.be/gfdwpuo-x5s
**скоріше за всі підтримується вхід тільки через QR код**, для повноцінного функціонування інших засобів входу необхідно мати повноцінний браузер. Ви можете спробувати, якщо буде працювати - пишіть мені. 



4. Завантаження протоколів на сайт.
Відео, як обрати теку з документами https://youtu.be/H77itkHDPxY 
- Необхідно натиснути кнопку **Select folder with documents** та обрати теку, у якій знаходяться попередньо сформовані протоколи. Як формувати протоколи ви зможете прочитати нижче.
- Після вибору теки, якщо там щось буде знайдено, у вікні з'явиться сформований висновок з обробки наданих документів. Незабаром буде все локалізовано.
- Якщо є документи, які можна відправити - необхідно натиснути кнопку **Start**

# Формування теки з документами.

Документ - це тека з файлами, які буде завантажено на сайт. Приклад, як виглядає тека на старті - 
![alt](https://bdfss.blob.core.windows.net/shots/2019-04-20_19-31-30-d01bd813-be96-4614-bebe-68ae893909df-explorer.png)
Назва теки має наступний формат:
(НОМЕР ОВК)-(НОМЕР ДІЛЬНИЦІ)-(ПОРЯДКОВИЙ НОМЕР ДОКУМЕНТУ)-(D), який складається з номеру ОВК, номеру дільниці, яка відноситься до ОВК, порядкового номеру документа, наприклад, якщо у вас є кілька _різних_ протоколів - наприклад, а D - якщо це уточнений протокол. Цей параметр буде додано у роботу дещо пізніше, і я його жодного разу не використовував, але нехай буде.
**Наводжу приклади**

- 192-680187 - простий документ, який відноситься до дільниці _680187_ у _192 ОВК_
- 192-680187-1 та 192-680187-2 - два _різні_ протоколи з однієї дільниці
- 192-680187-D - уточнений протокол, на сайт передається додатковий параметр, через що на цьому документі буде мітка **уточнений**.
- 192-680187-1-D та 192-680187-2-D - два уточнених _різних_ протоколи
 

# Генерація тек для ОВК - як сгенерувати документи під фотографії протоколів для ТВО.
Відео з оглядом процесу створення робочих тек для ТВО - https://youtu.be/RwceQJY6XLY
  - для пришвидшення створення тек під документи буде додано додатковий таб **Create-Documents** у котрому ви зможете сгенерувати всі теки для обраного вами ОВК, документи будуть сгенеровані у місці старту додатку, наприклад, якщо ви обрали 192 ОВК, буде створено теку з назвою **output**, у якій будуть всі теки-документи з цього ОВК - "192-680187", "192-680188" і так далі. 

__УВАГА - не використовуйте створені теки у результаті роботи программи, їх необхідно скопіювати у інше місце, бо при наступній генерації программа видалить попередньо сгенеровані папки__
