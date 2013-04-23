Регерация сертификата и его установка

SET COMPUTER_NAME=localhost

генерируем сертификат 
makecert.exe -sr LocalMachine -ss My -n CN=%COMPUTER_NAME% -sky exchange -sk -pe
makecert.exe -sr LocalMachine -ss My -n CN=localhost -sky exchange -sk -pe

устанавливаем
certmgr.exe -add -r LocalMachine -s My -c -n %COMPUTER_NAME% -r CurrentUser -s TrustedPeople
certmgr.exe -add -r LocalMachine -s My -c -n localhost -r CurrentUser -s TrustedPeople


Не административные аккаунты по умолчанию не имеют права открывать HTTP соединения на прослушивание

Выдача прав
в Vista, Windows 7, Windows 2008+
netsh http add urlacl url=http://+:80/MyUri user=DOMAIN\user

в XP, Windows 2003
httpcfg set urlacl /u http://+:80/MyUri /a "O:DAG:DAD:(A;;GRGX;;;DA)(A;;GA;;;BA)"


логика покупки билета:
1) List<Seat> SeatsGet(TripSchedule ts) - получаем список мест на рейс (маршрут+время) с флагами "занят/свободен/оформляется"
2) TimeSpan SeatLock(Seat seat) - блокировка места с тайм-аутом в 10 минут(настраивается в конфиге) для оформления
3) вносим данные пассажира
4) void (?) SeatSale(Seat seat, Passenger passenger) - продажа билета - заносятся в базу данные пассажира, разблокируется место, печатается чек.

Прописать в базу:
1) при удалении городов проверяем маршруты

Прописать в модели:
1) ГосНомер автобуса - аа 000 64RUS. Допустимые буквы - АВЕКМНОРСТУХ (русские)
2) ОГРН перевозчика - 13 цифр. 
		122334444445
			1 - вид. 1, 5, 2 или 3
			2 - год внесения записи в реестр
			3 - субъект РФ (ст 64 конституции РФ)
			4 - номер
			5 - контрольная цифра - проверить
3) строки забить длину и проверку на "только пробелы"