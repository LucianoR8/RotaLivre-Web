import { useState, useEffect } from 'react';
import { AdminUser, Category, Tour, Review, ActiveScreen } from './types';
import { Header } from './components/Header';
import { Sidebar } from './components/Sidebar';
import { LoginScreen } from './components/LoginScreen';
import { DashboardScreen } from './components/DashboardScreen';
import { CategoriesList } from './components/CategoriesList';
import { CategoryForm } from './components/CategoryForm';
import { ToursList } from './components/ToursList';
import { TourForm } from './components/TourForm';
import { DeleteConfirmModal } from './components/DeleteConfirmModal';
import { ToastContainer, ToastMessage } from './components/Toast';

// Importando nossos novos serviços!
import { categoriaService } from './services/categoriaService';
import { passeioService } from './services/passeioService';

export default function App() {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(() => !!localStorage.getItem('admin_token'));
  const [currentUser, setCurrentUser] = useState<AdminUser | null>(() => {
    const savedUser = localStorage.getItem('admin_user');
    return savedUser ? JSON.parse(savedUser) : null;
  });

  const [activeScreen, setActiveScreen] = useState<ActiveScreen>('dashboard');
  const [mobileSidebarOpen, setMobileSidebarOpen] = useState(false);

  const [categories, setCategories] = useState<Category[]>([]);
  const [tours, setTours] = useState<Tour[]>([]);
  const [reviews, setReviews] = useState<Review[]>([]);

  const [categoryToEdit, setCategoryToEdit] = useState<Category | null>(null);
  const [tourToEdit, setTourToEdit] = useState<Tour | null>(null);

  const [deleteModal, setDeleteModal] = useState<{ isOpen: boolean; type: 'category' | 'tour'; item: Category | Tour | null; }>({
    isOpen: false, type: 'category', item: null,
  });

  const [toasts, setToasts] = useState<ToastMessage[]>([]);

  const addToast = (type: 'success' | 'error' | 'info', title: string, description?: string) => {
    const id = Math.random().toString(36).substring(2, 9);
    setToasts((prev) => [...prev, { id, type, title, description }]);
    setTimeout(() => setToasts((prev) => prev.filter((t) => t.id !== id)), 4000);
  };

  const handleDismissToast = (id: string) => setToasts((prev) => prev.filter((t) => t.id !== id));

  // =========================================================================
  // CARREGAMENTO DOS DADOS (API)
  // =========================================================================
const carregarDadosDaApi = async () => {
  if (!isAuthenticated) return;

  try {
    const cats = await categoriaService.listar();
    const ps = await passeioService.listar();

    // Conta quantos passeios pertencem a cada categoria
    const catsComContagem = cats.map((categoria) => ({
      ...categoria,
      tourCount: ps.filter(
        (passeio) =>
          passeio.categoryId === categoria.id
      ).length,
    }));

    // Relaciona cada passeio com sua categoria
    const passeiosComCategoria = ps.map(
      (passeio) => {
        const categoria = cats.find(
          (cat) =>
            cat.id === passeio.categoryId
        );

        return {
          ...passeio,
          categoryName:
            categoria?.name ??
            'Sem categoria',
        };
      }
    );

    setCategories(catsComContagem);
    setTours(passeiosComCategoria);
  } catch (error) {
    console.error(
      'Erro ao carregar dados:',
      error
    );

    addToast(
      'error',
      'Erro de Conexão',
      'Não foi possível carregar os dados da API.'
    );
  }
};


  // Chama a API sempre que o usuário fizer login ou der F5 logado
  useEffect(() => {
    carregarDadosDaApi();
  }, [isAuthenticated]);


  // =========================================================================
  // AUTH
  // =========================================================================
  const handleLoginSuccess = (user: AdminUser) => {
    setIsAuthenticated(true);
    setCurrentUser(user);
    setActiveScreen('dashboard');
    addToast('success', `Bem-vindo, ${user.name}!`, 'Sessão administrativa iniciada.');
  };

  const handleLogout = () => {
    localStorage.removeItem('admin_token');
    localStorage.removeItem('admin_user');
    setIsAuthenticated(false);
    setCurrentUser(null);
    setActiveScreen('dashboard');
    addToast('info', 'Sessão encerrada', 'Você saiu do Painel Administrativo');
  };

  // =========================================================================
  // CATEGORIA CRUD
  // =========================================================================
  const handleStartCreateCategory = () => { setCategoryToEdit(null); setActiveScreen('category-new'); };
  const handleStartEditCategory = (cat: Category) => { setCategoryToEdit(cat); setActiveScreen('category-edit'); };

  const handleSaveCategory = async (categoryData: Partial<Category>) => {
    try {
      if (categoryToEdit) {
        await categoriaService.atualizar(categoryToEdit.id, categoryData);
        addToast('success', 'Categoria Atualizada', 'A categoria foi salva com sucesso no banco de dados.');
      } else {
        await categoriaService.criar(categoryData);
        addToast('success', 'Categoria Criada', 'A nova categoria já está disponível no app.');
      }
      setActiveScreen('categories');
      setCategoryToEdit(null);
      carregarDadosDaApi(); // Recarrega a lista
    } catch (error) {
      addToast('error', 'Erro ao Salvar', 'Ocorreu um erro ao comunicar com a API.');
    }
  };

  // =========================================================================
  // PASSEIO CRUD
  // =========================================================================
  const handleStartCreateTour = () => { setTourToEdit(null); setActiveScreen('tour-new'); };
  const handleStartEditTour = (tour: Tour) => { setTourToEdit(tour); setActiveScreen('tour-edit'); };

  const handleSaveTour = async (tourData: Partial<Tour>) => {
    try {
      if (tourToEdit) {
        await passeioService.atualizar(tourToEdit.id, tourData);
        addToast('success', 'Passeio Atualizado', 'As alterações foram salvas com sucesso.');
      } else {
        await passeioService.criar(tourData);
        addToast('success', 'Passeio Criado', 'O passeio já está visível para os usuários.');
      }
      setActiveScreen('tours');
      setTourToEdit(null);
      carregarDadosDaApi(); // Recarrega a lista
    } catch (error) {
      addToast('error', 'Erro ao Salvar', 'Ocorreu um erro ao comunicar com a API.');
    }
  };

  // =========================================================================
  // DELETE (AMBOS)
  // =========================================================================
  const handleRequestDelete = (type: 'category' | 'tour', item: Category | Tour) => {
    setDeleteModal({ isOpen: true, type, item });
  };

  const handleConfirmDelete = async () => {
    if (!deleteModal.item) return;

    try {
      if (deleteModal.type === 'category') {
        await categoriaService.deletar(deleteModal.item.id);
        addToast('info', 'Categoria Excluída', 'A categoria foi desativada do sistema.');
      } else {
        await passeioService.deletar(deleteModal.item.id);
        addToast('info', 'Passeio Excluído', 'O passeio foi removido do catálogo.');
      }
      carregarDadosDaApi(); // Recarrega a lista após deletar
    } catch (error) {
      addToast('error', 'Erro ao Excluir', 'Não foi possível excluir o item.');
    } finally {
      setDeleteModal({ isOpen: false, type: 'category', item: null });
    }
  };

  // Rendering...
  if (!isAuthenticated || !currentUser) {
    return (
      <>
        <LoginScreen onLoginSuccess={handleLoginSuccess} />
        <ToastContainer toasts={toasts} onDismiss={handleDismissToast} />
      </>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-800 flex">
      <Sidebar
        activeScreen={activeScreen}
        onNavigate={setActiveScreen}
        onLogout={handleLogout}
        categoriesCount={categories.length}
        toursCount={tours.length}
        mobileOpen={mobileSidebarOpen}
        onCloseMobile={() => setMobileSidebarOpen(false)}
      />

      <div className="flex-1 lg:pl-64 flex flex-col min-w-0">
        <Header user={currentUser} activeScreen={activeScreen} onNavigate={setActiveScreen} onToggleMobileSidebar={() => setMobileSidebarOpen(true)} />
        <main className="flex-1 p-4 sm:p-6 lg:p-8 max-w-7xl w-full mx-auto">
          {activeScreen === 'dashboard' && <DashboardScreen categories={categories} tours={tours} reviews={reviews} onNavigate={setActiveScreen} onEditTour={handleStartEditTour} onEditCategory={handleStartEditCategory} />}
          {activeScreen === 'categories' && <CategoriesList categories={categories} onNavigate={setActiveScreen} onEdit={handleStartEditCategory} onRequestDelete={(cat) => handleRequestDelete('category', cat)} />}
          {(activeScreen === 'category-new' || activeScreen === 'category-edit') && <CategoryForm categoryToEdit={categoryToEdit} onSave={handleSaveCategory} onCancel={() => { setCategoryToEdit(null); setActiveScreen('categories'); }} />}
          {activeScreen === 'tours' && <ToursList tours={tours} categories={categories} onNavigate={setActiveScreen} onEdit={handleStartEditTour} onRequestDelete={(tour) => handleRequestDelete('tour', tour)} />}
          {(activeScreen === 'tour-new' || activeScreen === 'tour-edit') && <TourForm tourToEdit={tourToEdit} categories={categories} adminUser={currentUser} onSave={handleSaveTour} onCancel={() => { setTourToEdit(null); setActiveScreen('tours'); }} />}
        </main>
      </div>

      <DeleteConfirmModal isOpen={deleteModal.isOpen} itemType={deleteModal.type} itemName={deleteModal.item?.name || ''} onConfirm={handleConfirmDelete} onCancel={() => setDeleteModal({ isOpen: false, type: 'category', item: null })} />
      <ToastContainer toasts={toasts} onDismiss={handleDismissToast} />
    </div>
  );
}