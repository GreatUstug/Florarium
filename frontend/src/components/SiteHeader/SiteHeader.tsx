import {Link, NavLink} from 'react-router';
import {ShoppingBag, User} from 'lucide-react';
import styles from './SiteHeader.module.css';

interface SiteHeaderProps {
    cartCount?: number;
}

const links = [
    {to: '/', label: 'каталог', end: true},
    {to: '/constructor', label: 'конструктор', end: false},
    {to: '/library', label: 'библиотека', end: false},
];

function SiteHeader({cartCount = 0}: SiteHeaderProps) {
    return (
        <header className={styles.header}>
            <div className={styles.inner}>
                <Link to="/" className={styles.logo}>
                    флорариум.
                </Link>

                <nav className={`nav ${styles.links}`} aria-label="основная навигация">
                    {links.map((link) => (
                        <NavLink key={link.to} to={link.to} end={link.end}>
                            {link.label}
                        </NavLink>
                    ))}
                    <a href="#delivery">доставка</a>
                </nav>

                <div className={`nav ${styles.actions}`}>
                    <NavLink to="/profile" className={styles.action}>
                        <User aria-hidden size="1.1em" strokeWidth={1.5}/>
                        профиль
                    </NavLink>
                    <NavLink to="/cart" className={styles.action}>
                        <ShoppingBag aria-hidden size="1.1em" strokeWidth={1.5}/>
                        корзина
                        <span className={styles.counter} aria-label={`позиций в корзине: ${cartCount}`}>
              {cartCount}
            </span>
                    </NavLink>
                </div>
            </div>
            <hr className="hr"/>
        </header>
    );
}

export {SiteHeader};
