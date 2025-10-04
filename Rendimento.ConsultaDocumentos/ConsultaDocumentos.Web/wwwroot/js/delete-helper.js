// Helper genérico para exclusão com confirmação via modal
(function () {
    let deleteId = null;
    let deleteUrl = null;
    let entityName = null;

    // Função para abrir o modal de confirmação
    window.openDeleteConfirmation = function (id, controller, itemDescription = '') {
        deleteId = id;
        deleteUrl = `/${controller}/DeleteConfirmed`;
        entityName = controller;

        // Atualizar informação do item no modal
        if (itemDescription) {
            document.getElementById('deleteItemInfo').textContent = itemDescription;
            document.getElementById('deleteItemInfo').style.display = 'block';
        } else {
            document.getElementById('deleteItemInfo').style.display = 'none';
        }

        // Abrir modal
        const modal = new bootstrap.Modal(document.getElementById('deleteConfirmationModal'));
        modal.show();
    };

    // Adicionar event listener para o botão de confirmação quando o documento estiver pronto
    document.addEventListener('DOMContentLoaded', function () {
        const confirmBtn = document.getElementById('confirmDeleteBtn');

        if (confirmBtn) {
            confirmBtn.addEventListener('click', function () {
                if (deleteId && deleteUrl) {
                    executeDelete();
                }
            });
        }
    });

    // Função para executar a exclusão via AJAX
    function executeDelete() {
        const confirmBtn = document.getElementById('confirmDeleteBtn');
        const originalBtnText = confirmBtn.textContent;

        // Desabilitar botão e mostrar loading
        confirmBtn.disabled = true;
        confirmBtn.textContent = 'Excluindo...';

        // Obter o token antiforgery
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        // Adicionar o ID como query string na URL
        const deleteUrlWithId = `${deleteUrl}?id=${deleteId}`;

        // Criar FormData com o token anti-forgery
        const formData = new FormData();
        if (token) {
            formData.append('__RequestVerificationToken', token);
        }

        fetch(deleteUrlWithId, {
            method: 'POST',
            body: formData
        })
        .then(response => {
            if (response.ok) {
                return response.json().catch(() => ({})); // Se não houver JSON, retorna objeto vazio
            } else {
                throw new Error('Erro ao excluir o registro');
            }
        })
        .then(data => {
            // Fechar modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('deleteConfirmationModal'));
            modal.hide();

            // Recarregar página para atualizar a lista
            window.location.reload();
        })
        .catch(error => {
            console.error('Erro:', error);
            alert('Erro ao excluir o registro. Por favor, tente novamente.');

            // Restaurar botão
            confirmBtn.disabled = false;
            confirmBtn.textContent = originalBtnText;
        });
    }
})();
