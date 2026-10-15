interface Screen {
    code: string;
    path: string;
    title: string;
}

const screens: Screen[] = [
    {code: '01', path: '/', title: 'каталог'},
    {code: '02', path: '/bouquet/:id', title: 'карточка букета'},
    {code: '03', path: '/constructor/:id?', title: 'конструктор букета'},
    {code: '04', path: '/cart', title: 'корзина'},
    {code: '05', path: '/checkout', title: 'оформление заказа'},
    {code: '06', path: '/profile', title: 'личный кабинет'},
    {code: '07', path: '/library', title: 'библиотека составов'},
    {code: '08', path: '/states', title: 'пустые и граничные состояния'},
];

export type {Screen};
export {screens};
