import {describe, expect, it} from 'bun:test';
import {render, screen} from '@testing-library/react';
import {MemoryRouter} from 'react-router';
import {SiteHeader} from './SiteHeader';

describe('SiteHeader', () => {
    it('показывает логотип, ссылки и счётчик корзины', () => {
        render(
            <MemoryRouter>
                <SiteHeader cartCount={3}/>
            </MemoryRouter>,
        );
        expect(screen.getByRole('link', {name: 'флорариум.'})).toHaveAttribute('href', '/');
        for (const label of ['каталог', 'конструктор', 'библиотека', 'доставка', 'профиль']) {
            expect(screen.getByRole('link', {name: new RegExp(label)})).toBeInTheDocument();
        }
        expect(screen.getByLabelText('позиций в корзине: 3')).toHaveTextContent('3');
    });
});
