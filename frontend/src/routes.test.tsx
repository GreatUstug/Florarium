import {describe, expect, it} from 'bun:test';
import {render, screen} from '@testing-library/react';
import {createMemoryRouter, RouterProvider} from 'react-router';
import {routes} from './routes';

function renderAt(url: string) {
    const router = createMemoryRouter(routes, {initialEntries: [url]});
    render(<RouterProvider router={router}/>);
}

describe('маршруты', () => {
    it.each([
        ['/', 'каталог'],
        ['/bouquet/42', 'карточка букета'],
        ['/constructor', 'конструктор букета'],
        ['/constructor/42', 'конструктор букета'],
        ['/cart', 'корзина'],
        ['/checkout', 'оформление заказа'],
        ['/profile', 'личный кабинет'],
        ['/library', 'библиотека составов'],
        ['/states', 'пустые и граничные состояния'],
    ])('%s показывает шапку и заголовок «%s»', (url, title) => {
        renderAt(url);
        expect(screen.getByRole('banner')).toBeInTheDocument();
        expect(screen.getByRole('heading', {level: 1})).toHaveTextContent(title);
    });

    it('неизвестный адрес ведёт на «страница не найдена»', () => {
        renderAt('/nope');
        expect(screen.getByRole('heading', {level: 1})).toHaveTextContent('страница не найдена');
    });
});
