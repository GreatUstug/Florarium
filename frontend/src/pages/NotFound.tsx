import {Link} from 'react-router';

function NotFound() {
    return (
        <section>
            <h1>страница не найдена</h1>
            <p>
                <Link to="/">вернуться в каталог</Link>
            </p>
        </section>
    );
}

export {NotFound};
