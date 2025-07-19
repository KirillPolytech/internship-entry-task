fetch('http://127.0.0.1:8080/Game/e6c0b69a-4afe-405d-b64a-89dd1e1bfb43/move') // Укажи актуальный путь к своему серверу
  .then(response => {
    if (!response.ok) throw new Error('Ошибка при получении данных');
    return response.json();
  })
  .then(data => {
    const symbols = {
      0: '',
      1: 'X',
      2: 'O'
    };

    const boardContainer = document.getElementById('board');
    const info = document.getElementById('info');
    boardContainer.innerHTML = ''; // Очистка перед перерисовкой

    const table = document.createElement('table');

    data.board.forEach(row => {
      const tr = document.createElement('tr');
      row.forEach(cell => {
        const td = document.createElement('td');
        td.textContent = symbols[cell];
        tr.appendChild(td);
      });
      table.appendChild(tr);
    });

    boardContainer.appendChild(table);

    info.innerHTML = `
      Статус: ${data.status === 0 ? 'Игра продолжается' : 'Игра завершена'}<br>
      Текущий игрок: ${symbols[data.currentPlayer] || '-'}<br>
      Победитель: ${data.winner !== 0 ? symbols[data.winner] : 'Пока нет'}
    `;
  })
  .catch(error => {
    const boardContainer = document.getElementById('board');
    boardContainer.textContent = 'Ошибка загрузки данных';
    console.error('Ошибка:', error);
  });
