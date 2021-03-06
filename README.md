﻿# Dice Gaming System
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
    Content-Type : application/json
    {
        "Username" : "testUsername",
        "Password" : "testPass123",
        "FullName" : "Test FullName",
        "Email" : "test@test.bg"
    }

    201 Created
	{
		"Id": "830238ec-cc12-4001-9c4b-b04405f85b2f",
		"UserName": "testUsername",
		"Email": "test@test.bg",
		"FullName": "Test FullName"
	}
    ```

2. Потребителите могат да се впишат (login) в системата. За целта те трябва да въведат своите потребителско име и парола.

    ```
    POST ~/Token
    Content-Type : application/x-www-form-urlencoded
    Username = testUsername
    Password = testPass123
    grant_type = password

    200 OK
    {
        "access_token": "4NzKFzWPS4itabRGENDFnHAfWbu_SmFjIwx7XeHOPkhrZek32l1FwhwFY4eQ8IxHuIvLDICwKmpVl6mDAS0XOAtDE0nAs4glXceuVhAqOfj1pkfFgoe-qp0hDkMM20zr4Zq2jorPOmUpX4qeO6MMNocmnJF3DpDEj22394FSToJmpQZ6s5vHYNoFqhaPR3d5LW2Y-hoR-6aQSTKcgbutSLJV6AEE1Ai7BxqFDjqZenGGAkICpZS8gxCQFQH-Dp95OHraHHyo8hbJPvp-xLrRQgFDrt3GUS8CzdDykFGrVse9tG3aOfVVZsj0GKsqZSsz4hOQA81VHWRD0SVMYVIW00Mv3Jau8TLhmgRfGEDi7SUPYgpNTvvsLJqbXiJitfaHnYF6HpM-Tgitw3TaWLX1eXHdqg3DHwBq5p0LYYaZAVAPBjYbu4CaoR8QA7QgAeAKQY9aNRSiEYxSLAMED0QFMBI4xsNsP2atNtzCE3pQlpM",
        "token_type": "bearer",
        "expires_in": 1209599,
        "userName": "testUsername",
        ".issued": "Sat, 12 Nov 2016 19:11:02 GMT",
        ".expires": "Sat, 26 Nov 2016 19:11:02 GMT"
    }
    ```

    Access token - a token that grants access to a resource.

    Bearer token - a particular type of access token, with the property that anyone can use the token. In other words, a client doesn’t need a cryptographic key or other secret to use a bearer token. For that reason, bearer tokens should only be used over a HTTPS, and should have relatively short expiration times.

    Кешираме "access_token" в session storage

    ![OAuth2](https://github.com/BiserSirakov/DiceGamingSystemApi/blob/master/oauth07.png)

3. Потребителите могат да се отпишат (logout) от системата.

    ```
    Потребителят не поставя header - Authorization : bearer "access_token" (Изтриваме/забравяме "access_token" от session storage)
    ```

4. Всеки потребител може да достъпи информацията само за своя профил. Виртуалната валута (виж по-долу) не трябва да се връща с профила.

    ```
    GET ~/api/Account/UserInfo
    Authorization : bearer "access_token"

    200 OK
    {
        "Username": "testUsername",
        "Email": "test@test.bg",
        "FullName": "Test FullName"
    }
    ```

5. Всеки потребител може да промени данните в своя профил (освен Username). За промяна на парола, е необходимо да се потвърди и старата парола.

    ```
    PUT ~/api/Account/ChangeUserInfo
    Authorization : bearer "access_token"
    Content-Type : application/json
    {
		"Email" : "test2@test.bg",
		"FullName" : "Test2 FullName"
	}

    204 No Content
    ```
	
	```
    PUT ~/api/Account/ChangePassword
    Authorization : bearer "access_token"
    Content-Type : application/json
    {
		"OldPassword" : "testPass123",
		"NewPassword" : "pwd",
		"ConfirmPassword" : "pwd"
	}

    204 No Content
    ```

6. Всеки потребител може да изтрие акаунта си. Това действие трябва да изтрие всички данни за потребителя, включително баланса от виртуалната му валута и история на хвърлянията му. Това действие изисква потвърждаване с парола!

	```
    DELETE ~/api/Account/DeleteUser
    Authorization : bearer "access_token"
    Content-Type : application/json
    {
		"Password" : "pwd"
	}

    204 No Content
    ```

7. Потребителя може да "зареди" акаунта си с произволно количество виртуална валута (цяло положително число).

	```
    PUT ~/api/Account/Wallet
    Authorization : bearer "access_token"
    Content-Type : application/json
    {
		"Amount" : "100"
	}

    200 OK
	{
		"VirtualMoney": "100"
	}
    ```

8. Всеки потребител може да достъпи баланса на виртуалната валута само за своя акаунт.

	```
    GET ~/api/Account/Wallet
    Authorization : bearer "access_token"

    200 OK
	{
		"VirtualMoney": "100"
	}
	```

9. Всеки потребител може да предизвика хвърляне на два зара със съответно залагане на възможна сума от точките. За целта се изпращат:
    - Bet - каква сума на точките на двата зара очаква да се падне
    - Stake - какво количество виртуална валута залага
    
    Като резултат трябва да се получи:
    - Идентификатор на хвърлянето
    - Win - количеството спечелена виртуална валута (може да е 0 при загуба)
	
	```
    POST ~/api/Shuffles
    Authorization : bearer "access_token"
    Content-Type : application/json
    {
		"Bet" : "7",
		"Stake" : "10"
	}

    201 Created
	{
		"Id": "b90f8d72-07eb-4673-a399-35b2f646948b",
		"UserId": "0cf64083-5e5b-4411-9ea7-b434142027da",
		"Bet": 7,
		"Stake": 10,
		"Result": 6,
		"Win": 0,
		"Timestamp": "2017-01-05T16:22:31.6569702+02:00"
	}
    ```
	
10. Потребителят може да отмени последното си хвърляне, стига то да е извършено не повече от минута назад във времето, като за това хвръляне не остава запис в системата и всички движения по баланса на играча също се отменят.

	```
    DELETE ~/api/Shuffles
    Authorization : bearer "access_token"

    204 No Content
    ```

11. Получаване на списък с хвърлянията за потребителя - странициран и с възможност за сортиране по време на хвърлянето по големина на залога (stake) и по спечелената сума валута (win). Да се добави и възможност за показване само на печелившите или губещите хвърляния. Във върнатата информация да се съдържа само време на хвърлянето (timestamp), заложена и спечелена сума.

	```
    Get ~/api/Shuffles?skip=3&take=2&orderBy=timestamp&filter=win
    Authorization : bearer "access_token"

    200 OK
	[
		{
			"Timestamp": "2017-01-06T14:32:23.743",
			"Stake": 10,
			"Win": 50
		},
		{
			"Timestamp": "2017-01-06T14:32:24.143",
			"Stake": 10,
			"Win": 50
		} 
	]
    ```

12. Достъпване на конкретно хвърляне, като освен посочените в т. 11 данни, се връщат и предвидената и реалната сума от точките на заровете.

	```
    Get ~/api/Shuffles/8EB1D138-9E2C-45DA-97DC-0312A04B63D3
    Authorization : bearer "access_token"

    200 OK
	{
		"Timestamp": "2017-01-06T14:32:23.743",
		"Stake": 10,
		"Win": 50,
		"Bet": 6,
		"Result": 8
	}
    ```

13. Пояснения:
    - Данните за името на потребителя (FullName) и ел. поща (Email) не са задължителни.
    - Всички действия, описани от т. 3 до т. 10, важат само за потребители, които са се оторизирали в системата (преминали през login).
    - Системата хвърля два стандартни шестстенни зара. Въпреки че връща само сумата от точките, тази сума трябва да е разпределена максимално близо до реалното. Случайно число между 2 и 12 не е това, което се търси!
    - Очаква се сами да определите (добре аргументирано) колко да се печели от всяка позната сума от точки. Ако играч познае, че ще се падне 2, трябва да печели повече ако познае, че ще се падне 5. Не забравяйте и печалбата за "къщата".
    
* Well designed and routed URIs
* Well designed DTOs
* Mеtadata in headers
* Filters/Handlers for metaoperations
* Response codes & Error codes
* Controller layer separation (controllers for entity)
* Business layer (where needed!)
* Error handling
* Well designed data model
* Data layer separation
* Stable data operations
* Builds and runs without errors
* Clean code
* Optimized operations
* Correct behavior
* Demonstration - end2end tests, GUI, SHELL UI, Postman, CURL or any other suitable ways to prove the service is working

Bonus:
* Idempotent operations (where needed)
* Deployed to Azure


