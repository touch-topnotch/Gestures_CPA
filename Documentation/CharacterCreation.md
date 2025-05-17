# Создание персонажа и способностей

Пайплайн разработки
-
1. Записать жесты в VR (скорее всего, этот шаг уже выполнен)
2. [Создаем персонажа](#создаем-персонажа) - модель, аватары, пивоты, привязка к компонентам
3. Создаем оружие - добавляем логику, крутим параметры, добавляем к персонажу
4. Тестим у себя на пк
    - Отображается ок?
    - Механики удобные?
5. Тестим в VR 
    - Отображается ок?
    - Есть особенности хендтрекинга, перемещения, восприятия?
6. Тестим в мультиплеере 
    - Как отображается у других? 
    - Работают ли кооп-механики (нерфы/баффы и т.д.)
7. Снимаем видос в очках (меньше одной минуты), на котором должно быть: 
    1. Появление жеста
    2. Использование оружия
    3. Нанесение урона по врагу
    4. Появление этого же жеста у врага
    5. Использование оружия
    6. Нанесение урона по нам



# Создаем персонажа

Добро пожаловать, мне кажется, в самое удобное создание персонажа в игре. Благодаря [CharacterCreatorWindow](../assets/Scripts/Static/CharacterCreatorWindow.cs) мы можем создать …. барабанная дробь … Character Creator Window!



<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Находится это добро по пути <strong>Tools -> CharacterCreator</strong></p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_1.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>

<div>
<p></p>
</div>

<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Итак, внутри этого окна есть необходимые поля - <strong> Character Name</strong> - имя персонажа типа PascalCase без нижних подчеркиваний. Например AngerGrief.
<strong>CharacterModel</strong> - нужно подробно разобрать здесь. 
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_2.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>

<div>
<p></p>
</div>

<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <strong>Weapons</strong> - здесь вы можете добавить любые виды оружий. У вас могут быть не готовы скрипты логики/дизайна, это ок, можно добавить их и позже. Главное - напишите имена всех оружий, так создадутся необходимые директории (Resources/Weapons/ИмяОружия/) и префабы.
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_3.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>




Теперь разберемся с [WeaponDesign](../assets/scripts/components/WeaponDesign.cs) и [WeaponLogic](../assets/scripts/Weapons/Weapon.cs). Первый и главный вопрос - зачем мы это разделили на два отдельных компонента. Ответ - чтобы у пользователей была возможность изменять дизайн оружия, добавлять партиклы, эффекты, анимации and so on, но при этом они никак не могли изменить поведение оружия, тем самым отменяя возможность ставить 99999 урона и читерить. Другими словами - WeaponDesign - фронт способности, WeaponLogic - его бэк.
![](source/cc_4.png)

Внутри класса Weapon есть вариант условий (Conditions) которые вам необходимо будет описать. Оружие срабатывает по логике - `onAbilityCalled` → `onHitCondition` → `onImpactCondition`, или говоря русским языком - Когда игрок скастовал все жесты (onAbilityCalled), смотрим, когда вызовется условия возможности ударить (OnHitCondition) (допустим, катана набрала скорость/мы нажали на курок) и после этого проверяем условие на нанесение урона (onImpactCondition) - допустим, когда лезвие вошло в коллайдер противника. Получается так, что нам необходимо описать данные условия один раз у каждого типа оружия. Далее мы можем их наследовать и не заморачиваться с описанием тех же параметров (Например, Melee->onImpactCondition = коллайдер лезвия внутри врага, а Hammer->onImpactCondition = человек попал в область рядом с ударом, однако остальное осталось прежним)

Стоит отметить, что Weapon Design имеет превосходный компонент [ResourcesProcessor](../assets/Scripts/components/ResourcesProcessor.cs). С ними вы подробнее познакомитесь в части автоматизации, т.к. он может буквально брать ресурсы из чата Resources в Tелеграме (а ведь туда могут скидывать и по 40 файлов всяких звуков и вфксов на каждого персонажа) и автоматически сортировать по папкам, логически добавляя их на персонажа.

Итак, последняя деталь - это [HandAppearance Config](../assets/scripts/characters/HandAppearance.cs). Тут вы выбираете 3 главных цвета персонажа и по ним компилятор соберет визуал для рук. (Самая волшебная вещь - у нас не создается дополнительных материалов => батчинг везде одинаковый. На сцене присутствуют 10 материалов рук (для команды 5*5) - настройки которых мы можем динамически изменять благодаря конфигу).

<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Выбираем цвета и жмем Create</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_5.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>
<p> </p>
<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Magic!</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_6.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
        <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="image-2.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>

Также мы можем настраивать разные варианты отображения на разных аватаров.
<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Допустим, local будет дружелюбным</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_7.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
         <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="image-2.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>
<p></p>
<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>A Enemy - более агрессивным</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_8.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
         <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="image-3.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>
<div style="display: flex; align-items: center;">
    <div style="flex: 1; display: flex; flex-direction: column; align-items: center;">
        <img src="source/cc_9.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>

</div>
<div style="flex: 1; text-align: center; ">
    <p>Ну и когда готово - жмем <strong>Bake</strong></p>
</div>



<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Вы можете посмотреть на созданного персонажа в папке Resources/Characters/ИмяПерсонажа</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_10.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>
<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>SrciptableObject CharData_ИмяПерсонажа должен выглядеть примерно так</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_11.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>
<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Последнее, что осталось сделать - убедиться, что ваш персонаж добавился в CharacterConfigs по пути Prefabs/Managers/CharacterController</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_13.png" alt="Description of the image" style="width: 100%; max-width: 400px;">
       </div>
</div>
<p></p>

# Ставим пивоты у персонажа

