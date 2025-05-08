Character And Abilities Creating
Добро пожаловать, мне кажется, в самое удобное создание персонажа в игре. Благодаря [CharacterCreatorWindow](../assets/Scripts/Static/CharacterCreatorWindow.cs) мы можем создать …. барабанная дробь … Character Creator Window!



<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Находится это добро по пути <strong>Tools -> CharacterCreator</strong></p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_1.png" alt="Description of the image" style="width: 100%; max-width: 700px;">
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
        <img src="source/cc_2.png" alt="Description of the image" style="width: 100%; max-width: 700px;">
       </div>
</div>

<div>
<p></p>
</div>

<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <strong>Weapons</strong> - здесь вы можете добавить любые виды оружий (самое главное, на любой стадии готовности, т.е. без дизайна/логики, c скриптом, но без методов или с ними. Самое главное - у вас автоматически создадутся и подтянутся все префабы оружий, добавятся к игроку и вручную ничего прокидывать не придется.
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_3.png" alt="Description of the image" style="width: 100%; max-width: 700px;">
       </div>
</div>




Теперь разберемся с [WeaponDesign](../assets/scripts/components/WeaponDesign.cs) и [WeaponLogic](../assets/scripts/Weapons/Weapon.cs). Первый и главный вопрос - зачем мы это разделили на два отдельных компонента. Ответ - чтобы у пользователей была возможность изменять дизайн оружия, добавлять партиклы, эффекты, анимации and so on, но при этом они никак не могли изменить поведение оружия, тем самым отменяя возможность ставить 99999 урона и читерить. Другими словами - WeaponDesign - фронт способности, WeaponLogic - его бэк.
![](source/cc_4.png)

Внутри класса Weapon есть вариант условий (Conditions) которые вам необходимо будет описать. Оружие срабатывает по логике - onAbilityCalled → onHitCondition → onImpactCondition, или говоря русским языком - Когда игрок скастовал все жесты, смотрим, когда вызовется условия возможности ударить (допустим, катана набрала скорость или мы нажали на курок) и после этого условия проверяем условие на нанесение урона - допустим, когда лезвие вошло в коллайдер противника.

Стоит отметить, что Weapon Design имеет превосходный компонент [ResourcesProcessor](../assets/Scripts/components/ResourcesProcessor.cs). С ними вы подробнее познакомитесь в части автоматизации, т.к. он может буквально брать ресурсы из чата Resources в телеграме (а ведь туда могут скидывать и по 40 файлов всяких звуков и вфксов на каждого персонажа) и автоматически сортировать по папкам, логически добавляя их на персонажа.

Итак, последняя деталь - это [HandAppearance Config](../assets/scripts/characters/HandAppearance.cs). Тут вы выбираете 3 главных цвета персонажа и по ним компилятор соберет визуал для рук. (Самая волшебная вещь - у нас не создается дополнительных материалов => батчинг везде одинаковый. На сцене присутствуют 10 материалов рук (для команды 5*5) - настройки которых мы можем динамически изменять благодаря конфигу).

<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Выбираем цвета и жмем Create</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_5.png" alt="Description of the image" style="width: 100%; max-width: 700px;">
       </div>
</div>
<p> </p>
<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Magic</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_6.png" alt="Description of the image" style="width: 100%; max-width: 700px;">
       </div>
</div>



Также мы можем настраивать разные варианты отображения на разных аватаров.
<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>Допустим, local будет дружелюбным</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_7.png" alt="Description of the image" style="width: 100%; max-width: 700px;">
       </div>
</div>
<p></p>
<div style="display: flex; align-items: center;">
    <div style="flex: 1; text-align: center; ">
        <p>A Enemy - более агрессивным</p>
    </div>
    <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-end;">
        <img src="source/cc_8.png" alt="Description of the image" style="width: 100%; max-width: 700px;">
       </div>
</div>

<div style="display: flex; align-items: center;">
    <div style="flex: 1; display: flex; flex-direction: column; align-items: center;">
        <img src="source/cc_9.png" alt="Description of the image" style="width: 100%; max-width: 700px;">
       </div>

</div>
<div style="flex: 1; text-align: center; ">
    <p>Ну и когда готово - жмем <strong>Bake</strong></p>
</div>

Вы можете посмотреть на созданного персонажа в папке Resources/Characters/ИмяПерсонажа:


![](source/cc_10.png)

![](source/cc_11.png)

![](source/cc_12.png)

![](source/cc_13.png)