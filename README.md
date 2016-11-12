# Dice Gaming System
Курсова работа за Уеб програмиране с Microsoft Azure и C#.NET, зимен семестър 2016/2017

Задачата е да се изгради система, която да позволява неангажираща игра на зарове за един човек.

## Условия и Api Reference
1. Потребителите на системата могат да се регистрират, като полетата които трябва да бъдат попълнени са:
    - Username - латински букви и цифри, уникално за системата
    - Password - латински букви, цифри и специални символи, между 6 и 20 символа
    - FullName
    - Email

    ```
    POST ~/api/Account/Register
    Content-Type : application/x-www-form-urlencoded
    Username = testUsername
    Password = testPass123!
    FullName = Test FullName
    Email = test@test.bg
    
    200 OK
    ```

2. Потребителите могат да се впишат (login) в системата. За целта те трябва да въведат своите потребителско име и парола.

3. Потребителите могат да се отпишат (logout) от системата.
4. Всеки потребител може да достъпи информацията само за своя профил. Виртуалната валута (виж по-долу) не трябва да се връща с профила.
5. Всеки потребител може да промени данните в своя профил (освен Username). За промяна на парола, е необходимо да се потвърди и старата парола.
6. Всеки потребител може да изтрие акаунта си. Това действие трябва да изтрие всички данни за потребителя, включително баланса от виртуалната му валута и история на хвърлянията му. Това действие изисква потвърждаване с парола!
7. Потребителя може да "зареди" акаунта си с произволно количество виртуална валута (цяло положително число).
8. Всеки потребител може да достъпи баланса на виртуалната валута само за своя акаунт.
9. Всеки потребител може да предизвика хвърляне на два зара със съответно залагане на възможна сума от точките. За целта се изпращат:
    - Bet - каква сума на точките на двата зара очаква да се падне
    - Stake - какво количество виртуална валута залага
    
    Като резултат трябва да се получи:
    - Идентификатор на хвърлянето
    - Win - количеството спечелена виртуална валута (може да е 0 при загуба)
10. Потребителят може да отмени последното си хвърляне, стига то да е извършено не повече от минута назад във времето, като за това хвръляне не остава запис в системата и всички движения по баланса на играча също се отменят.
11. Получаване на списък с хвърлянията за потребителя - странициран и с възможност за сортиране по време на хвърлянето по големина на залога (stake) и по спечелената сума валута (win). Да се добави и възможност за показване само на печелившите или губещите хвърляния. Във върнатата информация да се съдържа само време на хвърлянето (timestamp), заложена и спечелена сума.
12. Достъпване на конкретно хвърляне, като освен посочените в т. 11 данни, се връщат и предвидената и реалната сума от точките на заровете.

13. Пояснения:
    - Данните за името на потребителя (FullName) и ел. поща (Email) не са задължителни.
    - Всички действия, описани от т. 3 до т. 10, важат само за потребители, които са се оторизирали в системата (преминали през login).
    - Системата хвърля два стандартни шестстенни зара. Въпреки че връща само сумата от точките, тази сума трябва да е разпределена максимално близо до реалното. Случайно число между 2 и 12 не е това, което се търси!
    - Очаква се сами да определите (добре аргументирано) колко да се печели от всяка позната сума от точки. Ако играч познае, че ще се падне 2, трябва да печели повече ако познае, че ще се падне 5. Не забравяйте и печалбата за "къщата".
    
     

